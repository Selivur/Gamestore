using System.Globalization;
using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Services;

public class GameService : IGameService
{
    private readonly GamestoreContext _context;

    public GameService(GamestoreContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new game in the database.
    /// </summary>
    /// <param name="game">The game object to be created.</param>
    /// <returns>A tuple indicating whether the creation was successful and an error message if applicable.</returns>
    public async Task<(bool IsSuccess, string? ErrorMessage)> CreateGameAsync(Game game)
    {
        try
        {
            game.GameAlias = NormalizeGameAlias(game.GameAlias);

            if (await _context.Games.AnyAsync(g => g.GameAlias == game.GameAlias))
            {
                return (false, "Game alias must be unique");
            }

            _context.Games.Add(game);

            var saved = await _context.SaveChangesAsync();

            return saved > 0
                ? (true, null)
                : (false, "Failed to save changes to the database.");
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Database error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a game from the database by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to retrieve.</param>
    /// <returns>A tuple representing the result of the operation, including a boolean indicating success, an error message if applicable, and the retrieved game object.</returns>
    /// <remarks>If the game alias is null or empty, the method will return a tuple with a false success value and an error message. If the game is not found in the database, the method will return a tuple with a false success value and an error message. Otherwise, the method will return a tuple with a true success value and the retrieved game object.</remarks>
    public async Task<(bool IsSuccess, string? ErrorMessage, Game? Game)> GetGameByAliasAsync(string gameAlias)
    {
        try
        {
            if (string.IsNullOrEmpty(gameAlias))
            {
                return (false, "Game alias was null or empty", null);
            }

            gameAlias = NormalizeGameAlias(gameAlias);

            var game = await _context.Games.SingleOrDefaultAsync(g => g.GameAlias == gameAlias);

            return game == null
                ? (false, "Game not found", null)
                : (true, null, game);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    /// <summary>
    /// Updates an existing game in the database.
    /// </summary>
    /// <param name="game">The game object containing the updated properties.</param>
    /// <returns>A tuple indicating whether the update was successful and an error message if applicable.</returns>
    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateGameAsync(Game game)
    {
        if (string.IsNullOrWhiteSpace(game.GameAlias))
        {
            return (false, "Game alias cannot be null or empty");
        }

        game.GameAlias = NormalizeGameAlias(game.GameAlias);

        var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameAlias == game.GameAlias);

        if (existingGame == null)
        {
            return (false, "Game not found");
        }

        existingGame.Name = game.Name;
        existingGame.Description = game.Description;

        try
        {
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException ex)
        {
            return (false, ex.InnerException?.Message ?? ex.Message);
        }
    }

    /// <summary>
    /// Removes a game from the database based on its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to be removed.</param>
    /// <returns>A tuple indicating whether the removal was successful and an error message if applicable.</returns>
    public async Task<(bool IsSuccess, string? ErrorMessage)> RemoveGameAsync(string gameAlias)
    {
        gameAlias = NormalizeGameAlias(gameAlias);

        var game = await _context.Games.FirstOrDefaultAsync(g => g.GameAlias == gameAlias);

        if (game == null)
        {
            return (false, "Game not found");
        }

        _context.Games.Remove(game);

        try
        {
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException ex)
        {
            return (false, ex.InnerException?.Message ?? ex.Message);
        }
    }

    /// <summary>
    /// Retrieves all games from the database.
    /// </summary>
    /// <returns>An IEnumerable of Game objects.</returns>
    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        var games = await _context.Games.ToListAsync();
        return games;
    }

    /// <summary>
    /// Normalizes a game alias by replacing spaces with hyphens and converting to lowercase.
    /// </summary>
    /// <param name="name">The name to be normalized.</param>
    /// <returns>The normalized game alias.</returns>
    private static string NormalizeGameAlias(string name)
    {
        return name.Replace(" ", "-").ToLower(CultureInfo.InvariantCulture);
    }
}