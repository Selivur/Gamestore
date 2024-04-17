using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IGenreRepository"/> interface for interacting with genres in the database.
/// </summary>
public class GenreRepository : IGenreRepository
{
    private readonly GamestoreContext _context;
    private readonly IDataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for interacting with the underlying data store.</param>
    public GenreRepository(GamestoreContext context, IDataBaseLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Genre?> GetByNameAsync(string name)
    {
        return await _context.Genres.SingleOrDefaultAsync(g => g.Name.Equals(name));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _context.Genres.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Genre genre)
    {
        _context.Genres.Add(genre);

        await SaveChangesAsync("Error when adding the genre from the database.", CrudOperation.Add, null, genre.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Genre genre)
    {
        _context.Entry(genre).State = EntityState.Modified;

        var oldObject = await _context.Genres.AsNoTracking().FirstAsync(o => o.Id == genre.Id);

        await SaveChangesAsync("Error when updating the genre from the database.", CrudOperation.Update, oldObject.ToBsonDocument(), genre.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var genreToRemove = await _context.Genres.SingleOrDefaultAsync(g => g.Id.Equals(id))
            ?? throw new ArgumentException($"No genre found with id '{id}'.", nameof(id));

        if (genreToRemove.ParentId is null)
        {
            throw new DbUpdateException("Can't delete parent genre");
        }

        _context.Genres.Remove(genreToRemove);

        await SaveChangesAsync("Error when deleting the genre from the database.", CrudOperation.Delete, genreToRemove.ToBsonDocument(), null);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetByGameAliasAsync(string gameAlias)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.GameAlias == gameAlias);

        if (!gameExists)
        {
            throw new ArgumentException($"No game found with the alias '{gameAlias}'.", nameof(gameAlias));
        }

        var genreList = await _context.Games
            .Where(g => g.GameAlias == gameAlias)
            .SelectMany(g => g.Genres)
            .ToListAsync();

        return genreList;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetByParentIdAsync(int id)
    {
        return await _context.Genres
            .Where(g => g.ParentId == id)
            .ToListAsync();
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
