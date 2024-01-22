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
    /// Adds an order with details for the specified game alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game for which the order is being added.</param>
    /// <returns>A task representing the asynchronous operation, returning an <see cref="OrderBuyResponse"/>.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the specified game alias is not found or there are not enough games in the store.
    /// </exception>
    /// <remarks>
    /// If there is an existing open order, the method either adds the game to the existing order details or creates a new order.
    /// If there is no existing open order, a new order with details is created.
    /// </remarks>
    Task<OrderBuyResponse> AddOrderWithDetails(string gameAlias);

    /// <summary>
    /// Retrieves the cart details for a given order ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <returns>
    /// A collection of <see cref="CartDetailsDTO"/> representing the cart details for the specified order.
    /// </returns>
    Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId);

    /// <summary>
    /// Retrieves a list of all available payment methods.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns a list of payment methods.</returns>
    /// <remarks>
    /// The returned list contains <see cref="PaymentDetails"/> objects, each representing the details of a specific payment method.
    /// </remarks>
    Task<List<PaymentDetails>> GetAllPaymentMethods();

    /// <summary>
    /// Generates a PDF document for a bank invoice based on the provided order information.
    /// </summary>
    /// <returns>A byte array representing the generated PDF document.</returns>
    /// <remarks>
    /// The generated PDF contains the following information:
    /// - User ID
    /// - Order ID
    /// - Validity Date (default validity period is 3 days, configurable)
    /// - Sum.
    /// </remarks>
    /// <returns>A byte array containing the generated PDF document.</returns>
    Task<byte[]> GetBankPDFAsync();

    /// <summary>
    /// Gets the details of the open order, if available.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, returning a collection of <see cref="CartDetailsDTO"/> representing the open order details.
    /// </returns>
    /// <remarks>
    /// This method retrieves the open order and its details from the order repository.
    /// If no open order is found, an empty collection is returned.
    /// </remarks>
    Task<IEnumerable<CartDetailsDTO>> GetOpenOrderDetailsAsync();
}