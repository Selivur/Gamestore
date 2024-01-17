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
    public DateTime? OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the total price of the order.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the collection of order details associated with the order.
    /// </summary>
    public ICollection<OrderDetails> OrderDetails { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with the order.
    /// </summary>
    public Customer Customer { get; set; }
}
