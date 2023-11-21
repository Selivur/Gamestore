using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Represents an interface for interacting with a genre repository.
/// </summary>
public interface IGenreRepository
{
    /// <summary>
    /// Retrieves a genre by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the game to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved genre.</returns>
    Task<Genre?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a genre by its name asynchronously.
    /// </summary>
    /// <param name="name">The name of the genre to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved genre.</returns>
    Task<Genre?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all genres in the repository asynchronously.
    /// </summary>
    /// <returns>An asynchronous task that returns a collection of all genres in the repository.</returns>
    Task<IEnumerable<Genre>> GetAllAsync();

    /// <summary>
    /// Adds a new genre to the repository asynchronously.
    /// </summary>
    /// <param name="genre">The genre to add to the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to add the game.</returns>
    Task AddAsync(Genre genre);

    /// <summary>
    /// Updates an existing genre in the repository asynchronously.
    /// </summary>
    /// <param name="genre">The genre to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to update the genre.</returns>
    Task UpdateAsync(Genre genre);

    /// <summary>
    /// Removes a game from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="name">The unique alias of the game to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to remove the genre.</returns>
    Task RemoveAsync(string name);
}
