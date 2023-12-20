using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Represents an interface for interacting with a order repository.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves a order by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <returns>An asynchronous task that returns the retrieved order.</returns>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all games in the repository asynchronously.
    /// </summary>
    /// <returns>An asynchronous task that returns a collection of all games in the repository.</returns>
    Task<IEnumerable<Order>> GetAllAsync();

    /// <summary>
    /// Adds a new order to the repository asynchronously.
    /// </summary>
    /// <param name="order">The order to add to the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to add the order.</returns>
    Task AddAsync(Order order);

    /// <summary>
    /// Updates an existing order in the repository asynchronously.
    /// </summary>
    /// <param name="order">The order to update in the repository.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to update the order.</returns>
    Task UpdateAsync(Order order);

    /// <summary>
    /// Removes a order from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The id of the order to remove.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing an error when failed to remove the order.</returns>
    /// <exception cref="ArgumentException">Thrown if no order is found with the specified company name.</exception>
    Task RemoveAsync(int id);
}