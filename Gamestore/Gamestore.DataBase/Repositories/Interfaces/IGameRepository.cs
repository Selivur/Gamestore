using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Represents an interface for interacting with a game repository.
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// Retrieves a game by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the game to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved game.</returns>
    Task<Game?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a game by its alias asynchronously.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved game.</returns>
    Task<Game?> GetByAliasAsync(string gameAlias);

    /// <summary>
    /// Retrieves all games in the repository asynchronously.
    /// </summary>
    /// <returns>An asynchronous task that returns a collection of all games in the repository.</returns>
    Task<IEnumerable<Game>> GetAllAsync();

    /// <summary>
    /// Adds a new game to the repository asynchronously.
    /// </summary>
    /// <param name="game">The game to add to the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to add the game.</returns>
    Task AddAsync(Game game);

    /// <summary>
    /// Updates an existing game in the repository asynchronously.
    /// </summary>
    /// <param name="game">The game to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to update the game.</returns>
    Task UpdateAsync(Game game);

    /// <summary>
    /// Removes a game from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="gameAlias">The unique alias of the game to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to remove the game.</returns>
    /// <exception cref="ArgumentException">Thrown if no game is found with the specified game alias.</exception>
    Task RemoveAsync(string gameAlias);

    /// <summary>
    /// Adds a new game to the database.
    /// </summary>
    /// <param name="game">The Game object containing the game data.</param>
    /// <param name="genresId">Array of genre IDs for the game.</param>
    /// <param name="platformsId">Array of platform IDs for the game.</param>
    /// <param name="publisherId">Publisher ID for the game.</param>
    Task AddGameWithDependencies(Game game, int[] genresId, int[] platformsId, int publisherId);

    /// <summary>
    /// Updates an existing game in the database.
    /// </summary>
    /// <param name="game">The Game object containing the updated game data.</param>
    /// <param name="genresId">Array of genre IDs for the game.</param>
    /// <param name="platformsId">Array of platform IDs for the game.</param>
    /// <param name="publisherId">Publisher ID for the game.</param>
    Task UpdateGameWithDependencies(Game game, int[] genresId, int[] platformsId, int publisherId);

    /// <summary>
    /// Asynchronously gets a list of games by publisher name.
    /// </summary>
    /// <param name="name">The name of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of games.</returns>
    Task<IEnumerable<Game>> GetByPublisherNameAsync(string name);

    /// <summary>
    /// Asynchronously gets a list of games by genre ID.
    /// </summary>
    /// <param name="id">The ID of the genre.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of games.</returns>
    Task<IEnumerable<Game>> GetByGenreIdAsync(int id);

    /// <summary>
    /// Asynchronously gets a list of games by platform type.
    /// </summary>
    /// <param name="type">The type of the platform.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of games.</returns>
    Task<IEnumerable<Game>> GetByPlatformTypeAsync(string type);
}