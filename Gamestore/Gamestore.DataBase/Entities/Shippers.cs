using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities;
public class Shippers
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("ShipperID")]
    public int ShipperID { get; set; }

    public string CompanyName { get; set; }

    public string Phone { get; set; }
}
