namespace Gamestore.Database.Entities;

/// <summary>
/// Represents an order details entity in the Gamestore database.
/// </summary>
public class OrderDetails
{
    /// <summary>
    /// Gets or sets the unique identifier of the order details.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the game in the order details.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price of the game in the order details.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the game in the order details.
    /// </summary>
    public int Discount { get; set; }

    /// <summary>
    /// Gets or sets the game associated with the order details.
    /// </summary>
    public Game Game { get; set; }
}
