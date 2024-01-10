using Gamestore.Api.Models.DTO.OrderDTO;

namespace Gamestore.Api.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Gets a Order by its id.
    /// </summary>
    /// <param name="id">The name of the Order.</param>
    /// <returns>The detailed information about the Order.</returns>
    Task<OrderResponse?> GetOrderByIdAsync(int id);

    /// <summary>
    /// Updates an existing Order in the database.
    /// </summary>
    /// <param name="order">The Order object containing the updated properties.</param>
    Task UpdateOrderAsync(OrderRequest order);

    /// <summary>
    /// Removes a Order from the database based on its name.
    /// </summary>
    /// <param name="id">The id of the Order to be removed.</param>
    /// <returns>A tuple indicating whether the removal was successful and an error message if applicable.</returns>
    Task RemoveOrderAsync(int id);

    /// <summary>
    /// Retrieves all orders from the database.
    /// </summary>
    /// <returns>An IEnumerable of Order objects.</returns>
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();

    /// <summary>
    /// Adds a new order to the database with the specified game details and updates an existing order if it already exists.
    /// </summary>
    /// <param name="order">The order details to be added or updated.</param>
    /// <param name="gameAlias">The alias of the game to be associated with the order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddOrderWithDetails(OrderRequest order, string gameAlias);

    /// <summary>
    /// Retrieves the cart details for a given order ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <returns>
    /// A collection of <see cref="CartDetailsDTO"/> representing the cart details for the specified order.
    /// </returns>
    Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId);
}