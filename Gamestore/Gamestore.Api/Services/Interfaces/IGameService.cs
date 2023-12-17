using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Game;

namespace Gamestore.Api.Services.Interfaces;

public interface IGameService
{
    /// <summary>
    /// Adds a new game to the database.
    /// </summary>
    /// <param name="fullGame">The GameWrapper object containing the game data and its relationships.</param>
    Task CreateGameAsync(GameWrapper fullGame);

    /// <summary>
    /// Retrieves a game from the database by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to retrieve.</param>
    /// <returns>The retrieved game object.</returns>
    /// <remarks>If the game alias is null or empty, the method will return a tuple with a false success value and an error message. If the game is not found in the database, the method will return a tuple with a false success value and an error message. Otherwise, the method will return a tuple with a true success value and the retrieved game object.</remarks>
    Task<GameResponse?> GetGameByAliasAsync(string gameAlias);

    /// <summary>
    /// Gets a game by its id.
    /// </summary>
    /// <param name="id">The id of the game.</param>
    /// <returns>The detailed information about the game.</returns>
    Task<GameResponse?> GetGameByIdAsync(int id);

    /// <summary>
    /// Updates an existing game in the database.
    /// </summary>
    /// <param name="fullGame">The GameWrapper object containing the updated game data and its relationships.</param>
    Task UpdateGameAsync(GameWrapper fullGame);

    /// <summary>
    /// Removes a game from the database based on its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to be removed.</param>
    /// <returns>A tuple indicating whether the removal was successful and an error message if applicable.</returns>
    Task RemoveGameAsync(string gameAlias);

    /// <summary>
    /// Retrieves all games from the database.
    /// </summary>
    /// <returns>An IEnumerable of Game objects.</returns>
    Task<IEnumerable<GameResponse>> GetAllGamesAsync();
}