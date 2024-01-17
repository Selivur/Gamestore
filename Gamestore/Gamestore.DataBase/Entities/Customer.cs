namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a customer entity in the Gamestore database.
/// </summary>
public class Customer
{
    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string Name { get; set; }
}
