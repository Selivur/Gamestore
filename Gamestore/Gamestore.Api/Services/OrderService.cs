using System.Collections.ObjectModel;
using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
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
    private readonly IOrderRepository _sqlOrderRepository;
    private readonly IMongoOrderRepository _mongoOrderRepository;
    private readonly IGameService _gameService;
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="sqlOrderRepository">The order repository providing data access for the service.</param>
    public OrderService(IOrderRepository sqlOrderRepository, IMongoOrderRepository mongoOrderRepository, IGameService gameRepository, IPaymentService paymentService)
    {
        _sqlOrderRepository = sqlOrderRepository;
        _mongoOrderRepository = mongoOrderRepository;
        _gameService = gameRepository;
        _paymentService = paymentService;
    }

    /// <inheritdoc/>
    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        var order = await _sqlOrderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Order not found");
        return OrderResponse.FromOrder(order);
    }

    /// <inheritdoc/>
    public async Task UpdateOrderAsync(OrderRequest order)
    {
        var existingOrder = await _sqlOrderRepository.GetByIdAsync(Convert.ToInt32(order.Id))
            ?? throw new KeyNotFoundException("Can't find the Order with this id");

        await _sqlOrderRepository.UpdateAsync(existingOrder);
    }

    /// <inheritdoc/>
    public async Task RemoveOrderAsync(int id)
    {
        await _sqlOrderRepository.RemoveAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _sqlOrderRepository.GetAllAsync();

        var orderResponses = orders.Select(OrderResponse.FromOrder).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OrderResponse>> GetOrdersBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        var sqlOrders = await _sqlOrderRepository.GetAllAsync();
        var mongoOrders = await _mongoOrderRepository.GetAllAsync();

        var allOrders = sqlOrders.Concat(mongoOrders);

        var filteredOrders = allOrders
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .ToList();

        var orderResponses = filteredOrders.Select(OrderResponse.FromOrder).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<OrderBuyResponse> AddOrderWithDetails(string gameAlias)
    {
        if (string.IsNullOrEmpty(gameAlias))
        {
            throw new ArgumentException("Game alias can't be null or empty.", nameof(gameAlias));
        }

        var openedOrder = await _sqlOrderRepository.GetFirstOpenOrderAsync();
        var game = await UpdateGameQuantityAndGetGameInfo(gameAlias);
        OrderBuyResponse response;

        if (openedOrder != null)
        {
            var existingOrderDetails = FindExistingOrderDetails(openedOrder, gameAlias);
            response = AddExistingOrderDetailsOrUpdateExistingOne(openedOrder, existingOrderDetails, game);
        }
        else
        {
            response = await CreateNewOrderandOrderDetails(game);
        }

        return response;
    }

    /// <inheritdoc/>
    public async Task<CreateOrderDTO> GetAllPaymentMethodsWithOrder()
    {
        var details = new CreateOrderDTO()
        {
            PaymentMethods = GetAllPaymentMethods(),

            Order = await _sqlOrderRepository.GetFirstOpenOrderAsync()
            ?? throw new KeyNotFoundException("Can't find any open orders"),
        };

        return details;
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetBankPDFAsync(PaymentRequestDTO paymentRequest)
    {
        var openOrder = await _sqlOrderRepository.GetFirstOpenOrderAsync();
        var order = await _sqlOrderRepository.GetByIdWithOrderDetailsAsync(openOrder.Id);

        await _sqlOrderRepository.CompleteOrder();

        using MemoryStream ms = new();
        using (var writer = new PdfWriter(ms).SetSmartMode(true))
        using (var pdf = new PdfDocument(writer))
        {
            using var document = new Document(pdf);

            var sum = order.OrderDetails
                .Sum(details => details.Quantity * details.Price * (1 - (details.Discount / 100d)));

            document.Add(new Paragraph($"Payment method: {paymentRequest.Method}"));
            document.Add(new Paragraph($"User ID: {order.Customer.Id}"));
            document.Add(new Paragraph($"Order ID: {order.Id}"));
            document.Add(new Paragraph($"Validity Date: {DateTime.Now.AddDays(3)}"));
            document.Add(new Paragraph($"Sum: {sum}"));
            document.Close();
        }

        return ms.ToArray();
    }

    /// <inheritdoc/>
    public async Task RemoveOrderDetailsAsync(string gameAlias)
    {
        var order = await _sqlOrderRepository.GetFirstOpenOrderAsync();

        var orders = await _sqlOrderRepository.GetAllOrderDetails(order.Id);

        var orderDetails = orders.FirstOrDefault(o => o.Game.GameAlias == gameAlias)
            ?? throw new KeyNotFoundException($"Can't find order details for the game alias '{gameAlias}' " +
            $"in the open order.");

        await RestockGameFromOrderDetailsAsync(gameAlias, orderDetails);

        await _sqlOrderRepository.RemoveOrderDetailsAsync(orderDetails.Id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetOpenOrderDetailsAsync()
    {
        var order = await _sqlOrderRepository.GetFirstOpenOrderAsync();

        var orders = await _sqlOrderRepository.GetAllOrderDetails(order.Id);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CartDetailsDTO>> GetCartDetailsAsync(int orderId)
    {
        var orders = await _sqlOrderRepository.GetAllOrderDetails(orderId);

        var orderResponses = orders.Select(CartDetailsDTO.FromOrderDetails).ToList();

        return orderResponses;
    }

    /// <inheritdoc/>
    public async Task<IBoxResponseDTO> GetIBoxTerminalOrderDetailsAsync()
    {
        var response = await _paymentService.ProcessIboxPayment();

        return response;
    }

    /// <inheritdoc/>
    public async Task<IBoxResponseDTO> GetVisaOrderDetailsAsync(VisaTransactionDTO model)
    {
        var response = await _paymentService.ProcessVisaPayment(model);

        return response;
    }

    /// <summary>
    /// Retrieves a collection of payment methods with their details.
    /// </summary>
    /// <returns>A collection of <see cref="PaymentDetails"/> representing different payment methods.</returns>
    /// <remarks>
    /// This method creates and returns a collection of <see cref="PaymentDetails"/> objects, each representing a different payment method.
    /// The collection includes the title, image URL, and description for each payment method.
    /// </remarks>
    private static ICollection<PaymentDetails> GetAllPaymentMethods()
    {
        List<PaymentDetails> paymentMethods = new()
        {
            new()
            {
                Title = "Bank",
                ImageUrl = "https://www.svgrepo.com/show/533463/bank.svg",
                Description = "is a financial institution that accepts deposits from the public and creates " +
                    "a demand deposit while simultaneously making loans",
            },
            new()
            {
                Title = "IBox terminal",
                ImageUrl = "https://www.svgrepo.com/show/315644/terminal.svg",
                Description = "also known as a point of sale (POS) terminal, credit card machine, card reader," +
                    " PIN pad, EFTPOS terminal",
            },
            new()
            {
                Title = "Visa",
                ImageUrl = "https://www.svgrepo.com/show/473823/visa.svg",
                Description = "is an American multinational payment card services corporation.",
            },
        };

        return paymentMethods;
    }

    /// <summary>
    /// Adds an order with its details in the system.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to add to the order.</param>
    /// <returns>A task that represents the asynchronous operation. The task result returns an <see cref="OrderBuyResponse"/> that contains the details of the new or updated order.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when there is no enough count of the game with the specified gameAlias in the store.</exception>
    /// <remarks>
    /// This method will update the game's quantity in stock,
    /// and then it will either add a new order and its details for the game or update an existing one.
    /// If there is an existing open order, this method will find the order details based on the game alias
    /// and update the open order and its details. Otherwise, it will create a new order and its details.
    /// </remarks>
    private async Task<Game> UpdateGameQuantityAndGetGameInfo(string gameAlias)
    {
        var game = await _gameService.GetGameByAliasAsync(gameAlias);

        if (game.UnitInStock == 0)
        {
            throw new KeyNotFoundException("Not enough games in store");
        }

        game.UnitInStock -= 1;
        await _gameService.UpdateGameWithoutDependenciesAsync(game);
        return game;
    }

    /// <summary>
    /// Finds the existing order details for the specified game alias in an opened order.
    /// </summary>
    /// <param name="openedOrder">The opened order to search in.</param>
    /// <param name="gameAlias">The alias of the game to find in order details.</param>
    /// <returns>The OrderDetails object for the specified game alias if it exists; throws KeyNotFoundException otherwise.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no order details are found for the given game alias.</exception>
    /// <remarks>
    /// This method looks through order details of the given opened order to find the details related to the game with the specified alias.
    /// </remarks>
    private static OrderDetails FindExistingOrderDetails(Order openedOrder, string gameAlias)
    {
        return openedOrder.OrderDetails.FirstOrDefault(od => od.Game.GameAlias == gameAlias);
    }

    /// <summary>
    /// Adds new order details or updates existing one and returns OrderBuyResponse.
    /// </summary>
    /// <param name="openedOrder">The opened order to be modified.</param>
    /// <param name="existingOrderDetails">Existing order details to update. If this is null, new order details are added.</param>
    /// <param name="game">The game to be added or updated in order details.</param>
    /// <returns>The OrderBuyResponse for the updated or new order details.</returns>
    /// <remarks>
    /// This method checks if the existing order details are not null,
    /// if they are not null it increments the quantity and updates the order details.
    /// If they are null, it creates a new order details and updates the opened order
    /// with these new order details. After the update, it returns OrderBuyResponse for the operation.
    /// </remarks>
    private OrderBuyResponse AddExistingOrderDetailsOrUpdateExistingOne(Order openedOrder, OrderDetails? existingOrderDetails, Game game)
    {
        if (existingOrderDetails != null)
        {
            existingOrderDetails.Quantity += 1;
            _sqlOrderRepository.UpdateOrderDetailsAsync(existingOrderDetails);
        }
        else
        {
            openedOrder.OrderDetails?.Add(CreateOrderDetails(game));
            _sqlOrderRepository.UpdateAsync(openedOrder);
        }

        return ConstructOrderBuyResponse(openedOrder, game, existingOrderDetails);
    }

    /// <summary>
    /// Creates a new OrderDetails object for the specified game.
    /// </summary>
    /// <param name="game">The game for which to create the order details.</param>
    /// <returns>A new OrderDetails object with the specified game details.</returns>
    private static OrderDetails CreateOrderDetails(Game game)
    {
        return new OrderDetails
        {
            Quantity = 1,
            Price = game.Price,
            Discount = game.Discount,
            Game = game,
        };
    }

    /// <summary>
    /// Constructs an OrderBuyResponse object based on the provided order, game, and order details.
    /// </summary>
    /// <param name="openedOrder">The opened order object.</param>
    /// <param name="game">The game object.</param>
    /// <param name="existingOrderDetails">Optional order details. If provided, its quantity is used in the response.</param>
    /// <returns>An OrderBuyResponse with the calculated sum and order details.</returns>
    private static OrderBuyResponse ConstructOrderBuyResponse(Order openedOrder, Game game, OrderDetails? existingOrderDetails)
    {
        var response = new OrderBuyResponse
        {
            Id = openedOrder.Id.ToString(),
            OrderDate = openedOrder.OrderDate,
            ProductID = Convert.ToInt32(game.Id),
            ProductName = game.Name,
            Price = game.Price,
            CreationDate = openedOrder.OrderDate,
            Discount = game.Discount,
            PaidDate = openedOrder.PaymentDate,
            Quantity = existingOrderDetails != null ? existingOrderDetails.Quantity : 1,
        };

        response.Sum = response.Quantity * response.Price * response.Discount / 100;
        return response;
    }

    /// <summary>
    /// Creates a new order and its details, then persists the order in the repository.
    /// Returns an OrderBuyResponse for the new order.
    /// </summary>
    /// <param name="game">The game for which to create the order.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is an OrderBuyResponse for the new order.</returns>
    private async Task<OrderBuyResponse> CreateNewOrderandOrderDetails(Game game)
    {
        var orderDetails = CreateOrderDetails(game);
        var order = await CreateAndPersistNewOrder(orderDetails);

        var orders = await _sqlOrderRepository.GetAllAsync();
        var maxId = orders?.Any() == true ? orders.Max(od => od.Id) : 0;

        var response = new OrderBuyResponse
        {
            Id = (maxId + 1).ToString(),
            OrderDate = order.OrderDate,
            ProductID = Convert.ToInt32(game.Id),
            ProductName = game.Name,
            Price = game.Price,
            CreationDate = order.OrderDate,
            Discount = game.Discount,
            PaidDate = order.PaymentDate,
        };
        response.Sum = response.Quantity * response.Price * response.Discount / 100;
        return response;
    }

    /// <summary>
    /// Creates a new Order object with the specified order details,
    /// then adds the order to the repository and returns it.
    /// </summary>
    /// <param name="orderDetails">The order details to include in the new order.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the newly created Order object.</returns>
    private async Task<Order> CreateAndPersistNewOrder(OrderDetails orderDetails)
    {
        var order = new Order
        {
            OrderDate = DateTime.Now,
            OrderDetails = new Collection<OrderDetails> { orderDetails },
            Status = OrderStatus.Open,
        };
        await _sqlOrderRepository.AddAsync(order);
        return order;
    }

    /// <summary>
    /// Restocks the game units from the order details asynchronously.
    /// Finds the open order, retrieves all order details, finds the order detail with the given game alias,
    /// increments the unit in stock for the game, updates the game, and finally removes the order detail.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to restock.</param>
    /// <exception cref="KeyNotFoundException">Thrown when no order details for the given game alias can be found in the open order.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task RestockGameFromOrderDetailsAsync(string gameAlias, OrderDetails orderDetails)
    {
        var game = _gameService.GetGameByAliasAsync(gameAlias).Result;
        game.UnitInStock += orderDetails.Quantity;

        await _gameService.UpdateGameWithoutDependenciesAsync(game);
    }
}
