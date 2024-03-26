using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents a product shipper with its details.
/// </summary>
public class ProductShipper
{
    /// <summary>
    /// Gets or sets the unique identifier for the product shipper.
    /// </summary>
    [NotMapped]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the shipper ID.
    /// </summary>
    [Key]
    [Column("Id")]
    [BsonElement("ShipperID")]
    public int ShipperId { get; set; }

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    [BsonElement("CompanyName")]
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [BsonElement("Phone")]
    public string Phone { get; set; }
}
