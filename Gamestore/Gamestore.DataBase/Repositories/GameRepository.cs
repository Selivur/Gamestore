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
        return await _context.Games.Include(g => g.Comments).ToListAsync();
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
        game.Genres = _context.Genres.Where(g => genresId.Contains(g.Id)).ToList();
        game.Platforms = _context.Platforms.Where(p => platformsId.Contains(p.Id)).ToList();
        game.Publisher = _context.Publishers.Find(publisherId);

        _context.Games.Add(game);
        await SaveChangesAsync("Error when adding the game to the database.");
    }

    /// <inheritdoc />
    public async Task UpdateGameWithDependencies(Game game, int[] genresId, int[] platformsId, int publisherId)
    {
        game = _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .FirstOrDefault(g => g.Id == game.Id);

        game.Genres.Clear();
        var selectedGenres = _context.Genres.Where(g => genresId.Contains(g.Id)).ToList();
        foreach (var genre in selectedGenres)
        {
            game.Genres.Add(genre);
        }

        game.Platforms.Clear();
        var selectedPlatforms = _context.Platforms.Where(p => platformsId.Contains(p.Id)).ToList();
        foreach (var platform in selectedPlatforms)
        {
            game.Platforms.Add(platform);
        }

        game.Publisher = _context.Publishers.Find(publisherId);

        _context.Games.Update(game);

        await SaveChangesAsync("Error when updating the game in the database.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetByPublisherNameAsync(string name)
    {
        var exists = await _context.Publishers.AnyAsync(g => g.CompanyName == name);

        if (!exists)
        {
            throw new ArgumentException($"No publisher found with the name '{name}'.", nameof(name));
        }

        var gameList = await _context.Publishers
            .Where(g => g.CompanyName == name)
            .SelectMany(g => g.Games)
            .ToListAsync();

        return gameList;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetByGenreIdAsync(int id)
    {
        var exists = await _context.Genres.AnyAsync(g => g.Id == id);

        if (!exists)
        {
            throw new ArgumentException($"No genre found with the name '{id}'.", nameof(id));
        }

        var gameList = await _context.Genres
            .Where(g => g.Id == id)
            .SelectMany(g => g.Games)
            .ToListAsync();

        return gameList;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetByPlatformTypeAsync(string type)
    {
        var exists = await _context.Platforms.AnyAsync(g => g.Type == type);

        if (!exists)
        {
            throw new ArgumentException($"No publisher found with the name '{type}'.", nameof(type));
        }

        var gameList = await _context.Platforms
            .Where(g => g.Type == type)
            .SelectMany(g => g.Games)
            .ToListAsync();

        return gameList;
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
