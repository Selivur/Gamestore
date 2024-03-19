using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities;
public class MongoOrder
{
    [BsonElement("OrderID")]
    public int Id { get; set; }

    [BsonElement("CustomerID")]
    public string CustomerId { get; set; }

    [BsonElement("EmployeeID")]
    public int EmployeeID { get; set; }

    [BsonElement("OrderDate")]
    [BsonSerializer(typeof(CustomDateTimeDeserializer))]
    public DateTime OrderDate { get; set; }

    [BsonElement("RequiredDate")]
    [BsonSerializer(typeof(CustomDateTimeDeserializer))]
    public DateTime RequiredDate { get; set; }

    [BsonElement("ShippedDate")]
    [BsonSerializer(typeof(CustomNullableDateTimeDeserializer))]
    public DateTime? ShippedDate { get; set; }

    [BsonElement("ShipVia")]
    public int ShipVia { get; set; }

    [BsonElement("Freight")]
    public double Freight { get; set; }

    [BsonElement("ShipName")]
    public string ShipName { get; set; }

    [BsonElement("ShipAddress")]
    public string ShipAddress { get; set; }

    [BsonElement("ShipCity")]
    public string ShipCity { get; set; }

    [BsonElement("ShipRegion")]
    public string ShipRegion { get; set; }

    [BsonElement("ShipPostalCode")]
    public string ShipPostalCode { get; set; }

    [BsonElement("ShipCountry")]
    public string ShipCountry { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PrivateId { get; set; }
}
