using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents an order with its details.
/// </summary>
public class ProductOrder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductOrder"/> class.
    /// </summary>
    public ProductOrder()
    {
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [Key]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [BsonElement("OrderID")]
    public int OrderID { get; set; }

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    [BsonElement("CustomerID")]
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the employee ID.
    /// </summary>
    [BsonElement("EmployeeID")]
    public int EmployeeID { get; set; }

    /// <summary>
    /// Gets or sets the order date.
    /// </summary>
    [BsonElement("OrderDate")]
    [BsonSerializer(typeof(CustomDateTimeDeserializer))]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the required date.
    /// </summary>
    [BsonElement("RequiredDate")]
    [BsonSerializer(typeof(CustomDateTimeDeserializer))]
    public DateTime RequiredDate { get; set; }

    /// <summary>
    /// Gets or sets the shipped date.
    /// </summary>
    [BsonElement("ShippedDate")]
    [BsonSerializer(typeof(CustomNullableDateTimeDeserializer))]
    public DateTime? ShippedDate { get; set; }

    /// <summary>
    /// Gets or sets the ship via.
    /// </summary>
    [BsonElement("ShipVia")]
    public int ShipVia { get; set; }

    /// <summary>
    /// Gets or sets the freight.
    /// </summary>
    [BsonElement("Freight")]
    public double Freight { get; set; }

    /// <summary>
    /// Gets or sets the ship name.
    /// </summary>
    [BsonElement("ShipName")]
    public string ShipName { get; set; }

    /// <summary>
    /// Gets or sets the ship address.
    /// </summary>
    [BsonElement("ShipAddress")]
    public string ShipAddress { get; set; }

    /// <summary>
    /// Gets or sets the ship city.
    /// </summary>
    [BsonElement("ShipCity")]
    public string ShipCity { get; set; }

    /// <summary>
    /// Gets or sets the ship region.
    /// </summary>
    [BsonElement("ShipRegion")]
    public string ShipRegion { get; set; }

    /// <summary>
    /// Gets or sets the ship postal code.
    /// </summary>
    [BsonElement("ShipPostalCode")]
    public string ShipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the ship country.
    /// </summary>
    [BsonElement("ShipCountry")]
    public string ShipCountry { get; set; }
}
