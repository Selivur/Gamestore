using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Models.Wrappers.Game;
using Gamestore.Database.Entities;

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
    Task<GameResponse?> GetGameResponseByAliasAsync(string gameAlias);

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

    /// <summary>
    /// Asynchronously gets a list of game responses by publisher name.
    /// </summary>
    /// <param name="name">The name of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of game responses.</returns>
    Task<IEnumerable<GameResponse>> GetAllGamesByPublisherNameAsync(string name);

    /// <summary>
    /// Asynchronously gets a list of game responses by platform type ID.
    /// </summary>
    /// <param name="id">The ID of the platform type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of game responses.</returns>
    Task<IEnumerable<GameResponse>> GetAllGamesByPlatformTypeAsync(int id);

    /// <summary>
    /// Asynchronously gets a list of game responses by platform type.
    /// </summary>
    /// <param name="type">The type of the platform.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of game responses.</returns>
    Task<IEnumerable<GameResponse>> GetAllGamesByPlatformTypeAsync(string type);

    /// <summary>
    /// Updates a game entity without modifying its dependencies.
    /// </summary>
    /// <param name="game">The <see cref="Game"/> containing the updated game information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the game with the specified ID is not found.
    /// </exception>
    Task UpdateGameWithoutDependenciesAsync(Game game);

    /// <summary>
    /// Retrieves game from the database.
    /// </summary>
    /// <returns>Game objects.</returns>
    Task<Game> GetGameByAliasAsync(string gameAlias);

    /// <summary>
    /// Retrieves all comments associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A collection of comments associated with the game.</returns>
    Task<IEnumerable<CommentResponse>> GetCommentsByGameAliasAsync(string gameAlias);

    /// <summary>
    /// Adds a comment to a game specified by its alias.
    /// </summary>
    /// <param name="commentWrapper">The comment to be added.</param>
    /// <param name="gameAlias">The alias of the game to which the comment will be added.</param>
    /// <exception cref="KeyNotFoundException">Thrown when no game with the specified alias is found.</exception>
    Task AddCommentAsync(CommentWrapper commentWrapper, string gameAlias);

    /// <summary>
    /// Deletes a comment specified by its ID.
    /// </summary>
    /// <param name="commentId">The ID of the comment to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteComment(int commentId);

    /// <summary>
    /// Returns the possible durations for a ban.
    /// </summary>
    /// <returns>An array of strings representing the possible ban durations.</returns>
    public Task<string[]> GetBanDurationsAsync();

    /// <summary>
    /// Bans a user.
    /// </summary>
    /// <param name="userName">The name of the user to ban.</param>
    /// <param name="banDuration">The duration of the ban.</param>
    Task BanUserAsync(string userName, string banDuration);
}