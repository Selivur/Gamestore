using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;
public class ShipperRepository : IShipperRepository
{
    private readonly IMongoCollection<ProductShipper> _shippers;

    public ShipperRepository(IMongoCollection<ProductShipper> shippers)
    {
        _shippers = shippers;
    }

    public async Task<List<ProductShipper>> GetAllShippersAsync()
    {
        return await _shippers.Find(shipper => true).ToListAsync();
    }
}
