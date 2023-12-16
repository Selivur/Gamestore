using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IPlatformRepository"/> interface for interacting with genres in the database.
/// </summary>
public class PlatformRepository : IPlatformRepository
{
    private readonly GamestoreContext _context;

    public PlatformRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Platform?> GetByIdAsync(int id)
    {
        return await _context.Platforms.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Platform?> GetByNameAsync(string name)
    {
        return await _context.Platforms.SingleOrDefaultAsync(g => g.Type.Equals(name));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        return await _context.Platforms.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Platform platform)
    {
        _context.Platforms.Add(platform);

        await SaveChangesAsync("Error when adding the platform from the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Platform platform)
    {
        _context.Entry(platform).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the platform from the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var platformToRemove = await _context.Platforms.SingleOrDefaultAsync(g => g.Id.Equals(id))
                            ?? throw new ArgumentException($"No paltform found with id '{id}'.", nameof(id));

        _context.Platforms.Remove(platformToRemove);

        await SaveChangesAsync("Error when deleting the platform from the database.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetByGameAliasAsync(string gameAlias)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.GameAlias == gameAlias);

        if (!gameExists)
        {
            throw new ArgumentException($"No game found with the alias '{gameAlias}'.", nameof(gameAlias));
        }

        var platformList = await _context.Games
            .Where(g => g.GameAlias == gameAlias)
            .SelectMany(p => p.Platforms)
            .ToListAsync();

        return platformList;
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage)
    {
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }
    }
}
