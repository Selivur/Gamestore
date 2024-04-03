using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gamestore.Database.Services;

/// <summary>
/// Handles logging of entity changes.
/// </summary>
public class DataBaseLogger
{
    private readonly IMongoCollection<Log> _logCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBaseLogger"/> class.
    /// </summary>
    /// <param name="context">The MongoDB database.</param>
    public DataBaseLogger(MongoContext context)
    {
        _logCollection = context.Logs;
    }

    /// <summary>
    /// Logs an entity change.
    /// </summary>
    public void LogChange(CrudOperation action, string? entityType, BsonDocument oldObject, BsonDocument newObject)
    {
        var log = new Log
        {
            Date = DateTime.UtcNow,
            Action = action.ToString(),
            EntityType = entityType,
            OldObject = oldObject,
            NewObject = newObject,
        };

        _logCollection.InsertOne(log);
    }
}
