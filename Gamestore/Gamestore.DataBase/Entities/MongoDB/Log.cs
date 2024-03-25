using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents a log entry.
/// </summary>

public class Log
{
    /// <summary>
    /// Gets or sets the date of the log entry.
    /// </summary>
    [BsonElement("Date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the action of the log entry.
    /// </summary>
    [BsonElement("Action")]
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the entity type of the log entry.
    /// </summary>
    [BsonElement("EntityType")]
    public string EntityType { get; set; }

    /// <summary>
    /// Gets or sets the old version of the object.
    /// </summary>
    [BsonElement("OldObject")]
    public BsonDocument OldObject { get; set; }

    /// <summary>
    /// Gets or sets the new version of the object.
    /// </summary>
    [BsonElement("NewObject")]
    public BsonDocument NewObject { get; set; }
}
