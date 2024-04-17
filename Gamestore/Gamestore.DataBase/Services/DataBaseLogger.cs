using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gamestore.Database.Services;

/// <summary>
/// Handles logging of entity changes.
/// </summary>
public class DataBaseLogger : IDataBaseLogger
{
    private readonly IMongoCollection<Log> _logCollection;

    public DataBaseLogger()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("Northwind");

        _logCollection = database.GetCollection<Log>("logs");
    }

    /// <inheritdoc/>
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
