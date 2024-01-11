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
    [HttpPost("/game/{gameAlias}/buy")]
    public async Task<IActionResult> CreateGame([FromBody] OrderRequest order, string gameAlias)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

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

    [HttpGet("/payment")]
    public async Task<IActionResult> GetPaymentMethodInfo(string name)
    {
        var cartDetails = await _orderService.GetPaymentDetails(name);

        return Ok(cartDetails);
    }

    /// <summary>
    /// Returns a list of error messages from the ModelState object.
    /// </summary>
    /// <returns>A list of error messages.</returns>
    private List<string> GetErrorMessages()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(v => v.ErrorMessage)
            .ToList();
    }
}