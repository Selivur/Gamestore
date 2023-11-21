using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Represents an interface for interacting with a platform repository.
/// </summary>
public interface IPlatformRepository
{
    /// <summary>
    /// Retrieves a platform by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved platform.</returns>
    Task<Platform?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a platform by its name asynchronously.
    /// </summary>
    /// <param name="name">The name of the platform to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved platform.</returns>
    Task<Platform?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all platforms in the repository asynchronously.
    /// </summary>
    /// <returns>An asynchronous task that returns a collection of all platforms in the repository.</returns>
    Task<IEnumerable<Platform>> GetAllAsync();

    /// <summary>
    /// Adds a new platform to the repository asynchronously.
    /// </summary>
    /// <param name="platform">The platform to add to the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to add the game.</returns>
    Task AddAsync(Platform platform);

    /// <summary>
    /// Updates an existing platform in the repository asynchronously.
    /// </summary>
    /// <param name="platform">The platform to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to update the platform.</returns>
    Task UpdateAsync(Platform platform);

    /// <summary>
    /// Removes a platform from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="name">The unique alias of the platform to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to remove the genre.</returns>
    Task RemoveAsync(string name);
}