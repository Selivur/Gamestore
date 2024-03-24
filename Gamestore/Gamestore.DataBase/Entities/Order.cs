using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents an order entity in the Gamestore database.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the collection of order details associated with the order.
    /// </summary>
    public ICollection<OrderDetails> OrderDetails { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with the order.
    /// </summary>
    public Customer? Customer { get; set; } = new Customer { Name = "Ivan" };

    /// <summary>
    /// Gets or sets the Status of the order.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the Payment Date of the order.
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    public static Order FromMongoOrder(ProductOrder productOrder)
    {
        return new Order
        {
            Id = productOrder.OrderID,
            Customer = new Customer { Name = productOrder.CustomerId },
            OrderDate = productOrder.OrderDate,
        };
    }
}
