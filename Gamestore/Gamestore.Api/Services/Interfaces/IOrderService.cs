using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Database.Repositories.Interfaces;

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

    Task<IEnumerable<OrderResponse>> GetOrdersBetweenDatesAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Adds an order with its details in the system.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to add to the order.</param>
    /// <returns>A task that represents the asynchronous operation. The task result returns an <see cref="OrderBuyResponse"/> that contains the details of the new or updated order.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when there is no enough count of the game with the specified gameAlias in the store.</exception>
    /// <remarks>
    /// This method will update the game's quantity in stock,
    /// and then it will either add a new order and its details for the game or update an existing one.
    /// If there is an existing open order, this method will find the order details based on the game alias
    /// and update the open order and its details. Otherwise, it will create a new order and its details.
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

    Task<byte[]> GetBankPDFAsync(PaymentRequestDTO paymentRequest);

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

    /// <summary>
    /// Removes the order details associated with the specified game alias from the open order in the database.
    /// </summary>
    /// <param name="gameAlias">The alias of the game for which the order details should be removed.</param>
    /// <returns>A task representing the asynchronous operation of removing the order details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no order details are found for the specified game alias in the open order.</exception>
    /// <remarks>
    /// This method retrieves the open order and its details from the order repository.
    /// If the specified game alias is associated with any order details in the open order, those details are removed.
    /// If no order details are found for the specified game alias, a <see cref="KeyNotFoundException"/> is thrown.
    /// </remarks>
    Task RemoveOrderDetailsAsync(string gameAlias);

    /// <summary>
    /// Retrieves all payment methods along with the details of the first open order.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of getting payment methods and open order details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no open orders are found.</exception>
    /// <remarks>
    /// This method creates a <see cref="CreateOrderDTO"/> object containing the payment methods
    /// and the details of the first open order retrieved
    /// from the <see cref="IOrderRepository.GetFirstOpenOrderAsync"/> method. If no open orders are found,
    /// a <see cref="KeyNotFoundException"/> is thrown.
    /// </remarks>
    Task<CreateOrderDTO> GetAllPaymentMethodsWithOrder();

    /// <summary>
    /// Retrieves order details for payments made via iBox Terminal.
    /// </summary>
    /// <returns>The order details for iBox Terminal payments.</returns>
    Task<IBoxResponseDTO> GetIBoxTerminalOrderDetailsAsync();

    /// <summary>
    /// Retrieves order details for payments made via Visa.
    /// </summary>
    /// <param name="model">The Visa Transaction object.</param>
    /// <returns>The order details for Visa payments.</returns>
    Task<IBoxResponseDTO> GetVisaOrderDetailsAsync(VisaTransactionDTO model);
}