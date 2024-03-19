using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

public interface IShipperRepository
{
    Task<List<Shippers>> GetAllShippersAsync();
}