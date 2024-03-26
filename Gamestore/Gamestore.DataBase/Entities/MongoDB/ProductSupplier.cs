using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents a supplier with its details.
/// </summary>
public class ProductSupplier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductSupplier"/> class.
    /// </summary>
    public ProductSupplier()
    {
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets or sets the unique identifier for the supplier.
    /// </summary>
    [NotMapped]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the supplier ID.
    /// </summary>
    [Column("Id")]
    [BsonElement("SupplierID")]
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    [Key]
    [BsonElement("CompanyName")]
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    [BsonElement("ContactName")]
    public string ContactName { get; set; }

    /// <summary>
    /// Gets or sets the contact title.
    /// </summary>
    [BsonElement("ContactTitle")]
    public string ContactTitle { get; set; }

    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    [BsonElement("Address")]
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    [BsonElement("City")]
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    [BsonElement("Region")]
    public string Region { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    [BsonElement("PostalCode")]
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [BsonElement("Country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [BsonElement("Phone")]
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the fax number.
    /// </summary>
    [BsonElement("Fax")]
    public string Fax { get; set; }

    /// <summary>
    /// Gets or sets the home page.
    /// </summary>
    [BsonElement("HomePage")]
    public string HomePage { get; set; }
}
