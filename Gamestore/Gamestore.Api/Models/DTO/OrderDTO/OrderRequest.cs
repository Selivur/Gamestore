namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object (DTO) for creating or updating an order.
/// </summary>
public class OrderRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the customer's unique identifier for the order.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product in the order.
    /// </summary>
    public int ProductId { get; set; }

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
    /// Gets or sets the order date.
    /// </summary>
    public DateTime? OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the order.
    /// </summary>
    public int Discount { get; set; }
}