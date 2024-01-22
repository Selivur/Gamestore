using System.Collections.ObjectModel;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Gamestore.Api.Services;

/// <summary>
/// Implementation of the <see cref="IOrderService"/> interface.
/// Provides functionality for managing order-related operations.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGameService _gameRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="orderRepository">The order repository providing data access for the service.</param>
    public OrderService(IOrderRepository orderRepository, IGameService gameRepository)
    {
        _orderRepository = orderRepository;
        _gameRepository = gameRepository;
    }

    /// <inheritdoc/>
    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Order not found");
        return OrderResponse.FromOrder(order);
    }

    /// <inheritdoc/>
    public async Task UpdateOrderAsync(OrderRequest order)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(Convert.ToInt32(order.Id))
            ?? throw new KeyNotFoundException("Can't find the Order with this id");

        await _orderRepository.UpdateAsync(existingOrder);
    }

    /// <inheritdoc/>
    public async Task RemoveOrderAsync(int id)
    {
        await _orderRepository.RemoveAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        var orderResponses = orders.Select(OrderResponse.FromOrder).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<OrderBuyResponse> AddOrderWithDetails(string gameAlias)
    {
        var existingOrder = await _orderRepository.GetFirstOpenOrderAsync();

        var game = await _gameRepository.GetGameByAliasAsync(gameAlias)
                ?? throw new KeyNotFoundException("Not enough games in store");

        if (game.UnitInStock != 0)
        {
            game.UnitInStock -= 1;
        }
        else
        {
            throw new KeyNotFoundException("Not enough games in store");
        }

        if (existingOrder != null)
        {
            var existingOrderDetails = existingOrder.OrderDetails.FirstOrDefault(od => od.Game.GameAlias == gameAlias);

            OrderBuyResponse response = new()
            {
                Id = existingOrder.Id.ToString(),
                OrderDate = existingOrder.OrderDate,
                ProductID = Convert.ToInt32(game.Id),
                ProductName = game.Name,
                Price = game.Price,
                CreationDate = existingOrder.OrderDate,
                Discount = existingOrderDetails.Discount,
                PaidDate = existingOrder.PaymentDate,
            };

            if (existingOrderDetails != null)
            {
                existingOrderDetails.Quantity += 1;

                await _gameRepository.UpdateGameWithoutDependenciesAsync(GameRequest.FromGameResponse(game));
                await _orderRepository.UpdateOrderDetailsAsync(existingOrderDetails);

                response.Quantity = existingOrderDetails.Quantity;
            }
            else
            {
                OrderDetails orderDetails = new()
                {
                    Quantity = 1,
                    Price = game.Price,
                    Discount = game.Discount,
                    Game = game.ToGame(),
                };
                existingOrder.OrderDetails.Add(orderDetails);
                await _orderRepository.UpdateAsync(existingOrder);

                response.Quantity = 1;
            }

            response.Sum = response.Quantity * response.Price * response.Discount / 100;

            return response;
        }
        else
        {
            OrderDetails orderDetails = new()
            {
                Quantity = 1,
                Price = game.Price,
                Discount = game.Discount,
                Game = game.ToGame(),
            };
            Order order = new()
            {
                OrderDate = DateTime.Now,
                OrderDetails = new Collection<OrderDetails> { orderDetails },

                // Customer?
                Status = 0,
            };
            await _orderRepository.AddAsync(order);

            var orders = await _orderRepository.GetAllAsync();
            var maxId = orders.Max(od => od.Id);
            OrderBuyResponse response = new()
            {
                Id = (maxId + 1).ToString(),
                OrderDate = existingOrder.OrderDate,
                ProductID = Convert.ToInt32(game.Id),
                ProductName = game.Name,
                Price = game.Price,
                CreationDate = existingOrder.OrderDate,
                Discount = game.Discount,
                PaidDate = existingOrder.PaymentDate,
            };
            response.Sum = response.Quantity * response.Price * response.Discount / 100;

            return response;
        }
    }

    /// <inheritdoc/>
    public Task<List<PaymentDetails>> GetAllPaymentMethods()
    {
        List<PaymentDetails> paymentMethods = new()
        {
            new()
            {
                Title = "Bank",
                ImageUrl = "https://www.svgrepo.com/show/533463/bank.svg",
                Description = "is a financial institution that accepts deposits from the public and creates " +
                    "a demand deposit while simultaneously making loans. Lending activities can be directly performed by " +
                    "the bank or indirectly through capital markets.",
            },
            new()
            {
                Title = "IBox terminal",
                ImageUrl = "https://www.svgrepo.com/show/315644/terminal.svg",
                Description = "also known as a point of sale (POS) terminal, credit card machine, card reader," +
                    " PIN pad, EFTPOS terminal (or by the older term as PDQ terminal which stands for \"Process Data Quickly\")" +
                    ", is a device which interfaces with payment cards to make electronic funds transfers. The terminal typically " +
                    "consists of a secure keypad (called a PINpad) for entering PIN, a screen, a means of capturing information " +
                    "from payments cards and a network connection to access the payment network for authorization.",
            },
            new()
            {
                Title = "Visa",
                ImageUrl = "https://www.svgrepo.com/show/473823/visa.svg",
                Description = "is an American multinational payment card services corporation headquartered " +
                    "in San Francisco, California. It facilitates electronic funds transfers throughout the world, most " +
                    "commonly through Visa-branded credit cards, debit cards and prepaid cards.[5] Visa is one of the world's most " +
                    "valuable companies.",
            },
        };

        return Task.FromResult(paymentMethods);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetBankPDFAsync()
    {
        var openOrder = await _orderRepository.GetFirstOpenOrderAsync();
        var orderId = openOrder.Id;
        Order order = await _orderRepository.GetByIdWithOrderDetailsAsync(orderId);
        using MemoryStream ms = new();
        using (var writer = new PdfWriter(ms).SetSmartMode(true))
        using (var pdf = new PdfDocument(writer))
        {
            using var document = new Document(pdf);

            document.Add(new Paragraph($"User ID: {order.Customer.Id}"));
            document.Add(new Paragraph($"Order ID: {order.Id}"));
            document.Add(new Paragraph($"Validity Date: {DateTime.Now.AddDays(3)}"));
            document.Add(new Paragraph($"Sum: {order.Price}"));

            document.Close();
        }

        return ms.ToArray();
    }

    /// <inheritdoc/>
    public async Task RemoveOrderDetailsAsync(string gameAlias)
    {
        var order = await _orderRepository.GetFirstOpenOrderAsync();

        var orders = await _orderRepository.GetAllOrderDetails(order.Id);

        var orderDetails = orders.FirstOrDefault(o => o.Game.GameAlias == gameAlias)
            ?? throw new KeyNotFoundException($"Can't find order details for the game alias '{gameAlias}' in the open order.");

        await _orderRepository.RemoveOrderDetailsAsync(orderDetails.Id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetOpenOrderDetailsAsync()
    {
        var order = await _orderRepository.GetFirstOpenOrderAsync();

        var orders = await _orderRepository.GetAllOrderDetails(order.Id);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId)
    {
        var orders = await _orderRepository.GetAllOrderDetails(orderId);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }
}
