using Gamestore.Database.Entities.Enums;
using MongoDB.Bson;

namespace Gamestore.Database.Services.Interfaces;
public interface IDataBaseLogger
{
    /// <summary>
    /// Logs an entity change.
    /// </summary>
    public void LogChange(CrudOperation action, string? entityType, BsonDocument oldObject, BsonDocument newObject);
}
