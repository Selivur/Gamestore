using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents a product with its details.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    [NotMapped]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    [Key]
    [Column("Id")]
    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [BsonElement("ProductName")]
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the supplier ID.
    /// </summary>
    [BsonElement("SupplierID")]
    public int SupplierID { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [BsonElement("CategoryID")]
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the quantity per unit.
    /// </summary>
    [BsonElement("QuantityPerUnit")]
    public string QuantityPerUnit { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the units in stock.
    /// </summary>
    [BsonElement("UnitsInStock")]
    public int UnitsInStock { get; set; }

    /// <summary>
    /// Gets or sets the units on order.
    /// </summary>
    [BsonElement("UnitsOnOrder")]
    public int UnitsOnOrder { get; set; }

    /// <summary>
    /// Gets or sets the reorder level.
    /// </summary>
    [BsonElement("ReorderLevel")]
    public int ReorderLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is discontinued.
    /// </summary>
    [BsonElement("Discontinued")]
    public bool Discontinued { get; set; }

    [ForeignKey("SupplierID")]
    public ProductSupplier? Supplier { get; set; }

    [ForeignKey("CategoryID")]
    public ProductCategory? Category { get; set; }
}
