using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;

/// <summary>
/// API controller for managing order-related operations.
/// </summary>
[Route("orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderController"/> class.
    /// </summary>
    /// <param name="orderService">The order service used for handling order-related operations.</param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Gets the details of an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>An ActionResult containing the order details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of orders.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();

        return Ok(orders);
    }

    /// <summary>
    /// Creates a new order for a game purchase.
    /// </summary>
    /// <param name="order">The order details provided in the request body.</param>
    /// <param name="gameAlias">The alias of the game being purchased.</param>
    /// <returns>An ActionResult indicating the success of the operation.</returns>
    [HttpPut("/game/{gameAlias}/buy")]
    public async Task<IActionResult> CreateGame([FromBody] OrderRequest order, string gameAlias)
    {
        await _orderService.AddOrderWithDetails(order, gameAlias);

        return Ok();
    }

    /// <summary>
    /// Retrieves the details of the cart for a given order ID.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>Returns the cart details for the specified order.</returns>
    [HttpGet("/cart")]
    public async Task<IActionResult> GetCartDetails(int orderId)
    {
        var cartDetails = await _orderService.GetCartDetailsAsync(orderId);

        return Ok(cartDetails);
    }

    /// <summary>
    /// Retrieves payment method information based on the specified name.
    /// </summary>
    /// <param name="name">The name of the payment method.</param>
    /// <returns>An IActionResult containing the payment method information.</returns>
    [HttpGet("payment")]
    public async Task<IActionResult> GetPaymentMethodInfo(string name)
    {
        var cartDetails = await _orderService.GetPaymentDetails(name);

        return Ok(cartDetails);
    }

    /// <summary>
    /// Retrieves a bank invoice PDF for the specified order.
    /// </summary>
    /// <param name="orderId">The ID of the order for which the PDF is requested.</param>
    /// <param name="validityDays">The number of days the invoice is valid (default is 3 days).</param>
    /// <returns>A FileResult containing the bank invoice PDF.</returns>
    [HttpGet("{orderId}/invoice-pdf")]
    public async Task<IActionResult> GetBankInvoicePdf(int orderId, int validityDays = 3)
    {
        byte[] pdfBytes = await _orderService.GetBankPDFAsync(orderId, validityDays);

        return File(pdfBytes, "application/pdf", $"invoice_{orderId}.pdf");
    }
}