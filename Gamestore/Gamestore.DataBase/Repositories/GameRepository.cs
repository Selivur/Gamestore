using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IGameRepository"/> interface.
/// Provides data access functionality for managing game entities.
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for interacting with the underlying data store.</param>
    public GameRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _context.Games.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Game?> GetByAliasAsync(string gameAlias)
    {
        return await _context.Games.SingleOrDefaultAsync(g => g.GameAlias.Equals(gameAlias));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Game game)
    {
        _context.Games.Add(game);
        await SaveChangesAsync("Error when adding the game to the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Game game)
    {
        _context.Entry(game).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the game in the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string gameAlias)
    {
        var game = await _context.Games.SingleOrDefaultAsync(g => g.GameAlias.Equals(gameAlias))
            ?? throw new ArgumentException($"No game found with the Game Alias '{gameAlias}'.", nameof(gameAlias));

        _context.Games.Remove(game);

        await SaveChangesAsync("Error when deleting the game from the database.");
    }

    /// <inheritdoc />
    public async Task AddGameWithDependencies(Game game, int[] genresId, int[] platformsId, int publisherId)
    {
        game.Genre = _context.Genres.Where(g => genresId.Contains(g.Id)).ToList();
        game.Platforms = _context.Platforms.Where(p => platformsId.Contains(p.Id)).ToList();
        game.Publishers = _context.Publishers.Find(publisherId);

        _context.Games.Add(game);
        await SaveChangesAsync("Error when adding the game to the database.");
    }

    public async Task UpdateGameWithDependencies(Game game, int[] genresId, int[] platformsId, int publisherId)
    {
        game = _context.Games.Include(g => g.Genre).Include(g => g.Platforms)
        .FirstOrDefault(g => g.Id == game.Id);

        game.Genre.Clear();
        var selectedGenres = _context.Genres.Where(g => genresId.Contains(g.Id)).ToList();
        foreach (var genre in selectedGenres)
        {
            game.Genre.Add(genre);
        }

        // Handle the Platforms relationship
        game.Platforms.Clear();
        var selectedPlatforms = _context.Platforms.Where(p => platformsId.Contains(p.Id)).ToList();
        foreach (var platform in selectedPlatforms)
        {
            game.Platforms.Add(platform);
        }

        game.Publishers = _context.Publishers.Find(publisherId);

        _context.Games.Update(game);

        await SaveChangesAsync("Error when updating the game in the database.");
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
