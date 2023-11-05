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
    /// <returns>An asynchronous task representing the operation's completion.</returns>
    Task AddAsync(Game game);

    /// <summary>
    /// Updates an existing game in the repository asynchronously.
    /// </summary>
    /// <param name="game">The game to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion.</returns>
    Task UpdateAsync(Game game);

    /// <summary>
    /// Removes a game from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="gameAlias">The unique alias of the game to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion.</returns>
    Task RemoveAsync(string gameAlias);
}