using System.Collections.ObjectModel;
using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Implementation of the <see cref="IOrderService"/> interface.
/// Provides functionality for managing order-related operations.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="repository">The order repository providing data access for the service.</param>
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Order not found");
        return OrderResponse.FromOrder(order);
    }

    /// <inheritdoc/>
    public async Task UpdateOrderAsync(OrderRequest order)
    {
        var existingOrder = await _repository.GetByIdAsync(Convert.ToInt32(order.Id)) ?? throw new KeyNotFoundException("Can't find the Order with this id");

        await _repository.UpdateAsync(existingOrder);
    }

    /// <inheritdoc/>
    public async Task RemoveOrderAsync(int id)
    {
        await _repository.RemoveAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _repository.GetAllAsync();

        var orderResponses = orders.Select(OrderResponse.FromOrder).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task AddOrderWithDetails(OrderRequest order, string gameAlias)
    {
        var existingOrder = await _repository.GetByIdAsync(Convert.ToInt32(order.Id));

        Order newOrder = new()
        {
            OrderDate = order.OrderDate,
            Price = order.Sum,
            Customer = new Customer()
            {
                Id = Convert.ToInt32(order.CustomerId),
            },
            OrderDetails = new Collection<OrderDetails>()
            {
                new()
                {
                    Quantity = order.Quantity,
                    Price = order.Price,
                    Discount = order.Discount,
                },
            },
        };

        if (existingOrder != null)
        {
            newOrder.Id = existingOrder.Id;
            await _repository.UpdateGameWithDependencies(newOrder, gameAlias);
        }
        else
        {
            await _repository.AddOrderWithDependencies(newOrder, gameAlias);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId)
    {
        var orders = await _repository.GetAllOrderDetails(orderId);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }
}
