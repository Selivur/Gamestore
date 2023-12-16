using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IPublisherRepository"/> interface.
/// Provides data access functionality for managing publisher entities.
/// </summary>
public class PublisherRepository : IPublisherRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for interacting with the underlying data store.</param>
    public PublisherRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Publisher?> GetByIdAsync(int id)
    {
        return await _context.Publishers.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _context.Publishers.SingleOrDefaultAsync(g => g.CompanyName.Equals(name));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        return await _context.Publishers.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Publisher publisher)
    {
        _context.Publishers.Add(publisher);
        await SaveChangesAsync("Error when adding the publisher to the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Publisher publisher)
    {
        _context.Entry(publisher).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the publisher in the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string name)
    {
        var publisher = await _context.Publishers.SingleOrDefaultAsync(g => g.CompanyName.Equals(name))
            ?? throw new ArgumentException($"No publisher found with the company name '{name}'.", nameof(name));

        _context.Publishers.Remove(publisher);

        await SaveChangesAsync("Error when deleting the publisher from the database.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Publisher>> GetPublishersByGameAliasAsync(string gameAlias)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.GameAlias == gameAlias);

        if (!gameExists)
        {
            throw new ArgumentException($"No game found with the alias '{gameAlias}'.", nameof(gameAlias));
        }

        var publisherList = await _context.Publishers
            .Where(p => p.Games != null && p.Games.Any(g => g.GameAlias == gameAlias))
            .ToListAsync();

        return publisherList;
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
