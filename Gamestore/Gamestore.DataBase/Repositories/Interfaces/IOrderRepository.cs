using Gamestore.Database.Entities;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// Updates an existing order in the database with the specified game dependency.
    /// </summary>
    /// <param name="order">The order to be updated.</param>
    /// <param name="gameAlias">The alias of the game to be associated with the order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the game is not found.</exception>
    Task UpdateGameWithDependencies(Order order, string gameAlias);

    /// <summary>
    /// Adds a new order to the database with the specified game dependency.
    /// </summary>
    /// <param name="order">The order to be added.</param>
    /// <param name="gameAlias">The alias of the game to be associated with the order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the game or customer is not found.</exception>
    Task AddOrderWithDependencies(Order order, string gameAlias);

    /// <summary>
    /// Retrieves all order details for a given order ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <returns>
    /// A collection of <see cref="OrderDetails"/> representing the details of the specified order.
    /// </returns>
    Task<IEnumerable<OrderDetails>> GetAllOrderDetails(int orderId);

    /// <summary>
    /// Retrieves an order with its associated order details by the specified order ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>Returns the order with associated order details if found; otherwise, returns null.</returns>
    Task<Order?> GetByIdWithOrderDetailsAsync(int id);

    /// <summary>
    /// Asynchronously retrieves the first open order from the database.
    /// </summary>
    /// <returns>The first open order, or null if no open orders are found.</returns>
    Task<Order?> GetFirstOpenOrderAsync();

    /// <summary>
    /// Updates the details of an order in the database.
    /// </summary>
    /// <param name="orderDetails">The <see cref="OrderDetails"/> entity with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbUpdateException">
    /// Thrown when an error occurs while updating the order details in the database.
    /// </exception>
    Task UpdateOrderDetailsAsync(OrderDetails orderDetails);
}