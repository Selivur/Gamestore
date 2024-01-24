using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object for creating an order.
/// </summary>
public class CreateOrderDTO
{
    /// <summary>
    /// Gets or sets the order to be created.
    /// </summary>
    public Order Order { get; set; }

    /// <summary>
    /// Gets or sets the collection of payment methods.
    /// </summary>
    public ICollection<PaymentDetails> PaymentMethods { get; set; }
}