using System.Collections.ObjectModel;
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
        var existingOrder = await _repository.GetByIdAsync(Convert.ToInt32(order.Id))
            ?? throw new KeyNotFoundException("Can't find the Order with this id");

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
    public async Task<byte[]> GetBankPDFAsync(int orderId, int validityDays)
    {
        Order order = await _repository.GetByIdWithOrderDetailsAsync(orderId);
        using MemoryStream ms = new();
        using (var writer = new PdfWriter(ms).SetSmartMode(true))
        using (var pdf = new PdfDocument(writer))
        {
            using var document = new Document(pdf);

            document.Add(new Paragraph($"User ID: {order.Customer.Id}"));
            document.Add(new Paragraph($"Order ID: {order.Id}"));
            document.Add(new Paragraph($"Validity Date: {DateTime.Now.AddDays(validityDays)}"));
            document.Add(new Paragraph($"Sum: {order.Price}"));

            document.Close();
        }

        return ms.ToArray();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId)
    {
        var orders = await _repository.GetAllOrderDetails(orderId);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }
}
