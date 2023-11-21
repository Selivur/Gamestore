using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IGenreRepository"/> interface for interacting with genres in the database.
/// </summary>
public class GenreRepository : IGenreRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for interacting with the underlying data store.</param>
    public GenreRepository(GamestoreContext context)
    {
        _context = context;
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

        await SaveChangesAsync("Error when adding the game from the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Genre genre)
    {
        _context.Entry(genre).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the game from the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string name)
    {
        var genreToRemove = await _context.Genres.SingleOrDefaultAsync(g => g.Name.Equals(name)) ?? throw new InvalidOperationException("Genre not found");

        if (genreToRemove.ParentId is null)
        {
            throw new DbUpdateException("Can't delete parent genre");
        }

        _context.Genres.Remove(genreToRemove);

        await SaveChangesAsync("Error when deleting the game from the database.");
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
