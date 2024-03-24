using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents an order with its details.
/// </summary>
public class ProductOrderDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductOrderDetails"/> class.
    /// </summary>
    public ProductOrderDetails()
    {
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [Key]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [BsonElement("OrderID")]
    public int OrderID { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    [BsonElement("Quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the discount.
    /// </summary>
    [BsonElement("Discount")]
    public decimal Discount { get; set; }
}
