﻿using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IOrderRepository"/> interface.
/// Provides data access functionality for managing order entities.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly GamestoreContext _context;
    private readonly IDataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for interacting with the underlying data store.</param>
    public OrderRepository(GamestoreContext context, IDataBaseLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <inheritdoc />
    public async Task<Order?> GetFirstOpenOrderAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(od => od.OrderDetails)
                .ThenInclude(od => od.Game)
            .FirstOrDefaultAsync(o => o.Status == 0);
    }

    /// <inheritdoc />
    public async Task<Order?> GetByIdWithOrderDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(od => od.OrderDetails)
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

        await SaveChangesAsync("Error when adding the order to the database.", CrudOperation.Add, null, order.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;

        var oldOrder = await _context.Orders.AsNoTracking().FirstAsync(o => o.Id == order.Id);

        await SaveChangesAsync("Error when updating the order in the database.", CrudOperation.Update, oldOrder.ToBsonDocument(), order.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateOrderDetailsAsync(OrderDetails orderDetails)
    {
        _context.Entry(orderDetails).State = EntityState.Modified;

        var oldOrderDetails = await _context.Orders.AsNoTracking().FirstAsync(o => o.Id == orderDetails.Id);

        await SaveChangesAsync("Error when updating the order details in the database.", CrudOperation.Update, oldOrderDetails.ToBsonDocument(), orderDetails.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var order = await _context.Orders.SingleOrDefaultAsync(g => g.Id.Equals(id))
            ?? throw new ArgumentException($"No order found with the id '{id}'.", nameof(id));

        _context.Orders.Remove(order);

        await SaveChangesAsync("Error when deleting the order from the database.", CrudOperation.Delete, order.ToBsonDocument(), null);
    }

    /// <inheritdoc />
    public async Task RemoveOrderDetailsAsync(int id)
    {
        var order = await _context.OrderDetails.SingleOrDefaultAsync(g => g.Id.Equals(id))
            ?? throw new ArgumentException($"No order found with the id '{id}'.", nameof(id));

        _context.OrderDetails.Remove(order);

        await SaveChangesAsync("Error when deleting the order details from the database.", CrudOperation.Delete, order.ToBsonDocument(), null);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetails>> GetAllOrderDetails(int orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Game)
            .SelectMany(o => o.OrderDetails)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task CompleteOrder()
    {
        var order = await GetFirstOpenOrderAsync();

        var oldObject = (Order)order.Clone();

        order.Status = OrderStatus.Paid;
        order.PaymentDate = DateTime.Now;
        _context.Entry(order).State = EntityState.Modified;

        await SaveChangesAsync("Error when changed the order status to complete.", CrudOperation.Update, oldObject.ToBsonDocument(), order.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task CancelledOrder()
    {
        var order = await GetFirstOpenOrderAsync();

        var oldObject = (Order)order.Clone();

        order.Status = OrderStatus.Cancelled;
        _context.Entry(order).State = EntityState.Modified;

        await SaveChangesAsync("Error when changed the order status to cancelled.", CrudOperation.Update, oldObject.ToBsonDocument(), order.ToBsonDocument());
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage, CrudOperation operation, BsonDocument oldObject, BsonDocument newObject)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }

        _logger.LogChange(
            action: operation,
            entityType: typeof(ProductSupplier).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}
