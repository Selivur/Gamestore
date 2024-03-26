using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents an order with its details.
/// </summary>
public class ProductOrderDetails
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [NotMapped]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Key]
    [Column("Id")]
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
