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
    public async Task<IActionResult> GetAllOrders(DateTime start, DateTime end)
    {
        // TODO: add datetime serialisation (input string to DateTime)
        var orders = await _orderService.GetOrdersBetweenDatesAsync(start, end);

        return Ok(orders);
    }

    /// <summary>
    /// Creates a new order for a game purchase.
    /// </summary>
    /// <param name="gameAlias">The alias of the game being purchased.</param>
    /// <returns>An ActionResult indicating the success of the operation.</returns>
    [HttpGet("/game/{gameAlias}/buy")]
    public async Task<IActionResult> CreateGameOrder(string gameAlias)
    {
        var result = await _orderService.AddOrderWithDetails(gameAlias);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the details of the cart for a given order ID.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>Returns the cart details for the specified order.</returns>
    [HttpGet("/cart/{orderId}")]
    public async Task<IActionResult> GetCartDetails(int orderId)
    {
        var cartDetails = await _orderService.GetCartDetailsAsync(orderId);

        return Ok(cartDetails);
    }

    /// <summary>
    /// Retrieves the details of the open order and returns them as a response.
    /// </summary>
    /// <returns>
    /// A response containing the details of the open order in the form of a collection of <see cref="CartDetailsDTO"/>.
    /// </returns>
    /// <remarks>
    /// This HTTP GET endpoint is designed to be accessed at the route "/cart".
    /// It calls the <see cref="IOrderService.GetOpenOrderDetailsAsync"/> method to retrieve open order details asynchronously.
    /// If an open order is found, the details are returned in the response. If no open order is found, an empty response is returned.
    /// </remarks>
    [HttpGet("/cart")]
    public async Task<IActionResult> GetOpenOrderDetails()
    {
        var cartDetails = await _orderService.GetOpenOrderDetailsAsync();

        return Ok(cartDetails);
    }

    /// <summary>
    /// Retrieves payment method information based on the specified name.
    /// </summary>
    /// <returns>An IActionResult containing a list of payment method information.</returns>
    [HttpGet("payment")]
    public async Task<IActionResult> GetPaymentMethodInfo()
    {
        var cartDetails = await _orderService.GetAllPaymentMethodsWithOrder();

        return Ok(cartDetails);
    }

    /// <summary>
    /// Retrieves payment method information based on the specified name.
    /// </summary>
    /// <returns>An IActionResult containing a list of payment method information.</returns>
    [HttpDelete("remove/{gameAlias}")]
    public async Task<IActionResult> RemoveOrderDetails(string gameAlias)
    {
        await _orderService.RemoveOrderDetailsAsync(gameAlias);

        return Ok();
    }

    /// <summary>
    /// Retrieves a bank invoice PDF for the open order.
    /// </summary>
    /// <returns>A FileResult containing the bank invoice PDF.</returns>
    [HttpPost("invoice-pdf")]
    public async Task<IActionResult> GetBankInvoicePdf([FromBody] PaymentRequestDTO paymentRequest)
    {
        switch (paymentRequest.Method)
        {
            case "IBox terminal":
                var iBoxOrderDetails = await _orderService.GetIBoxTerminalOrderDetailsAsync();

                return Ok(iBoxOrderDetails);
            case "Visa":
                var visaOrderDetails = await _orderService.GetVisaOrderDetailsAsync(paymentRequest.Model
                    ?? throw new KeyNotFoundException("Missing Visa payment details"));

                return Ok(visaOrderDetails);
            case "Bank":
                byte[] pdfBytes = await _orderService.GetBankPDFAsync(paymentRequest);

                return File(pdfBytes, "application/pdf", $"invoice.pdf");
            default:
                throw new ArgumentException("Invalid payment method provided.", nameof(paymentRequest.Method));
        }
    }
}