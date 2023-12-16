using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Represents an interface for interacting with a publisher repository.
/// </summary>
public interface IPublisherRepository
{
    /// <summary>
    /// Retrieves a publisher by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved publisher.</returns>
    Task<Publisher?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a publisher by its alias asynchronously.
    /// </summary>
    /// <param name="name">The alias of the publisher to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved publisher.</returns>
    Task<Publisher?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all games in the repository asynchronously.
    /// </summary>
    /// <returns>An asynchronous task that returns a collection of all games in the repository.</returns>
    Task<IEnumerable<Publisher>> GetAllAsync();

    /// <summary>
    /// Adds a new publisher to the repository asynchronously.
    /// </summary>
    /// <param name="publisher">The publisher to add to the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to add the publisher.</returns>
    Task AddAsync(Publisher publisher);

    /// <summary>
    /// Updates an existing publisher in the repository asynchronously.
    /// </summary>
    /// <param name="publisher">The publisher to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to update the publisher.</returns>
    Task UpdateAsync(Publisher publisher);

    /// <summary>
    /// Retrieves a collection of publishers associated with a specified game alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>An asynchronous task that represents the operation, returning a collection of publishers.</returns>
    /// <exception cref="ArgumentException">Thrown if no game is found with the specified alias.</exception>
    Task<IEnumerable<Publisher>> GetPublishersByGameAliasAsync(string gameAlias);

    /// <summary>
    /// Removes a publisher from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="name">The unique alias of the publisher to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to remove the publisher.</returns>
    /// <exception cref="ArgumentException">Thrown if no publisher is found with the specified company name.</exception>
    Task RemoveAsync(string name);
}