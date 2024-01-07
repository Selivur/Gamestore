using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IOrderRepository"/> interface.
/// Provides data access functionality for managing order entities.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for interacting with the underlying data store.</param>
    public OrderRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.Include(o => o.Customer).ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await SaveChangesAsync("Error when adding the order to the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the order in the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var order = await _context.Orders.SingleOrDefaultAsync(g => g.Id.Equals(id))
            ?? throw new ArgumentException($"No order found with the id '{id}'.", nameof(id));

        _context.Orders.Remove(order);

        await SaveChangesAsync("Error when deleting the order from the database.");
    }

    /// <inheritdoc />
    public async Task AddOrderWithDependencies(Order order, string gameAlias)
    {
        var orderDetails = order.OrderDetails.First();

        orderDetails.Game = _context.Games.SingleOrDefault(g => g.GameAlias.Equals(gameAlias))
            ?? throw new KeyNotFoundException("Game not found with specified alias");

        order.Customer = _context.Customers.Find(order.Customer.Id)
            ?? throw new KeyNotFoundException("Customer not found");

        order.OrderDetails.Clear();
        order.OrderDetails.Add(orderDetails);

        _context.OrderDetails.Add(orderDetails);
        _context.Orders.Add(order);
        await SaveChangesAsync("Error when adding the order to the database.");
    }

    /// <inheritdoc />
    public async Task UpdateGameWithDependencies(Order order, string gameAlias)
    {
        var orderDetails = order.OrderDetails.First();

        var orderDetailsFromDb = _context.Orders.Include(g => g.OrderDetails).FirstOrDefault(g => g.Id == order.Id).OrderDetails;

        orderDetails.Game = _context.Games.SingleOrDefault(g => g.GameAlias.Equals(gameAlias))
            ?? throw new KeyNotFoundException("Game not found with specified alias");

        var orderDetailsToRemove = orderDetailsFromDb.FirstOrDefault(orderDetails => orderDetails.Game.GameAlias == gameAlias);

        if (orderDetailsToRemove != null)
        {
            orderDetailsFromDb.Remove(orderDetailsToRemove);
        }
        else
        {
            _context.OrderDetails.Add(orderDetails);
        }

        orderDetailsFromDb.Add(orderDetails);
        order.OrderDetails = orderDetailsFromDb;
        _context.Orders.Update(order);

        await SaveChangesAsync("Error when updating the order in the database.");
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }
    }
}
