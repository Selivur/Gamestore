namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents an order in the system.
/// </summary>
public class OrderBuyResponse
{
    /// <summary>
    /// Gets or sets the customer ID associated with the order.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the product ID associated with the order.
    /// </summary>
    public int ProductID { get; set; }

    /// <summary>
    /// Gets or sets the name of the product in the order.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in the order.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the total sum of the order.
    /// </summary>
    public int Sum { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the order.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the price of the product in the order.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the order.
    /// </summary>
    public int Discount { get; set; }

    /// <summary>
    /// Gets or sets the date when the order was paid, if applicable.
    /// </summary>
    public DateTime? PaidDate { get; set; }
}
