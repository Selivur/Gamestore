using Gamestore.Database.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Gamestore.Database.Dbcontext;
public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        if (client != null)
        {
            _database = client.GetDatabase(settings.Value.Database);
        }
    }

    public IMongoCollection<Shippers> Shippers
    {
        get { return _database.GetCollection<Shippers>("shippers"); }
    }

    public IMongoCollection<MongoOrder> Orders
    {
        get { return _database.GetCollection<MongoOrder>("orders"); }
    }
}
