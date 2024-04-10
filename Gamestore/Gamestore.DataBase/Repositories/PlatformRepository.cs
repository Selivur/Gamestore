using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IPlatformRepository"/> interface for interacting with genres in the database.
/// </summary>
public class PlatformRepository : IPlatformRepository
{
    private readonly GamestoreContext _context;
    private readonly DataBaseLogger _logger;

    public PlatformRepository(GamestoreContext context)
    {
        _context = context;
        _logger = new DataBaseLogger();
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

        await SaveChangesAsync("Error when adding the platform from the database.", CrudOperation.Add, null, platform.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Platform platform)
    {
        _context.Entry(platform).State = EntityState.Modified;

        var oldObject = await _context.Platforms.AsNoTracking().FirstAsync(o => o.Id == platform.Id);

        await SaveChangesAsync("Error when updating the platform from the database.", CrudOperation.Update, oldObject.ToBsonDocument(), platform.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var platformToRemove = await _context.Platforms.SingleOrDefaultAsync(g => g.Id.Equals(id))
                            ?? throw new ArgumentException($"No platform found with id '{id}'.", nameof(id));

        _context.Platforms.Remove(platformToRemove);

        await SaveChangesAsync("Error when deleting the platform from the database.", CrudOperation.Delete, platformToRemove.ToBsonDocument(), null);
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
    private async Task SaveChangesAsync(string errorMessage, CrudOperation operation, BsonDocument oldObject, BsonDocument newObject)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }

        _logger.LogChange(
            action: operation,
            entityType: typeof(ProductSupplier).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}
