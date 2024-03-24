using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;
public class ShipperRepository : IShipperRepository
{
    private readonly MongoContext _context;

    public ShipperRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<List<ProductShipper>> GetAllShippersAsync()
    {
        return await _context.Shippers.Find(shipper => true).ToListAsync();
    }
}
