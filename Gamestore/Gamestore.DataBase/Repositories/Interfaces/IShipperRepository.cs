using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Repositories.Interfaces;

public interface IShipperRepository
{
    Task<List<ProductShipper>> GetAllShippersAsync();
}