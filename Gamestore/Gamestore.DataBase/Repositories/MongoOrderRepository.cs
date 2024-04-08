using Gamestore.Database.Entities;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;
public class MongoOrderRepository : IMongoOrderRepository
{
    private readonly IMongoCollection<ProductOrder> _orders;

    public MongoOrderRepository(IMongoCollection<ProductOrder> orders)
    {
        _orders = orders;
    }

    public Task AddAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task CancelledOrder()
    {
        throw new NotImplementedException();
    }

    public Task CompleteOrder()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var orders = await _orders.Find(_ => true).ToListAsync();

        return orders.Select(Order.FromMongoOrder);
    }

    public Task<IEnumerable<OrderDetails>> GetAllOrderDetails(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> GetByIdWithOrderDetailsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> GetFirstOpenOrderAsync()
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveOrderDetailsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrderDetailsAsync(OrderDetails orderDetails)
    {
        throw new NotImplementedException();
    }
}
