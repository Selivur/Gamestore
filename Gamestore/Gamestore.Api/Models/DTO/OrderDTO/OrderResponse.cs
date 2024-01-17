using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object (DTO) for an order response.
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order date.
    /// </summary>
    public DateTime? OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the customer's unique identifier for the order.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Converts an <see cref="Order"/> object to an <see cref="OrderResponse"/> object.
    /// </summary>
    /// <param name="order">The <see cref="Order"/> object to convert.</param>
    /// <returns>A new instance of <see cref="OrderResponse"/> populated with data from the input <paramref name="order"/>.</returns>
    public static OrderResponse FromOrder(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.Customer.Id.ToString(),
        };
    }
}