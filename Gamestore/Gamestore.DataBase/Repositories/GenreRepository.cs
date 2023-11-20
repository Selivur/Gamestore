using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IGenreRepository"/> interface for interacting with genres in the database.
/// </summary>
internal class GenreRepository : IGenreRepository
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
    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _context.Genres.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Genre genre)
    {
        _context.Genres.Add(genre);
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when adding a genre to the database.");
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Genre genre)
    {
        _context.Entry(genre).State = EntityState.Modified;
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when adding a genre to the database.");
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var genreToRemove = await _context.Genres.FindAsync(id) ?? throw new InvalidOperationException("Genre not found");

        _context.Genres.Remove(genreToRemove);
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when deleting a game from the database.");
        }
    }
}
