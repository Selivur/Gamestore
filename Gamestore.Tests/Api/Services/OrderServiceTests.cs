using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="OrderService"/> class.
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IGameService> _mockGameService;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly IOrderService _orderService;
    private readonly Mock<MongoOrderRepository> _mockMongoOrderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderServiceTests"/> class.
    /// Setup for tests of the <see cref="OrderService"/> class.
    /// </summary>
    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockMongoOrderRepository = new Mock<MongoOrderRepository>();
        _mockGameService = new Mock<IGameService>();
        _mockPaymentService = new Mock<IPaymentService>();
        _orderService = new OrderService(_mockOrderRepository.Object, _mockMongoOrderRepository.Object, _mockGameService.Object, _mockPaymentService.Object);
    }

    /// <summary>
    /// Test for GetOrderByIdAsync method when the order does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void GetOrderByIdAsync_WhenOrderDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .Returns(Task.FromResult((Order)null));

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _orderService.GetOrderByIdAsync(It.IsAny<int>()));
        Assert.NotNull(exception.Result);
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for GetOrderByIdAsync method when the order exists.
    /// Expects order details to be returned.
    /// </summary>
    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        var testOrder = new Order { Status = OrderStatus.Open };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(testOrder);

        // Act
        var orderResponse = await _orderService.GetOrderByIdAsync(It.IsAny<int>());

        // Assert
        Assert.NotNull(orderResponse);
        Assert.Equal(testOrder.Id, orderResponse.Id);
        Assert.Equal(testOrder.OrderDate, orderResponse.OrderDate);
    }

    /// <summary>
    /// Test for AddOrderWithDetails method when the order request is valid.
    /// Expects order to be created successfully.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_ValidGameAlias_AddsOrderWithDetails()
    {
        // Arrange
        var testGameAlias = "validGameAlias";

        _mockOrderRepository
            .Setup(r => r.GetFirstOpenOrderAsync())
            .ReturnsAsync(default(Order))
            .Verifiable();

        _mockGameService
            .Setup(gs => gs.GetGameByAliasAsync(testGameAlias))
            .Returns(Task.FromResult(new Game { UnitInStock = 1 }))
            .Verifiable();

        // Act
        await _orderService.AddOrderWithDetails(testGameAlias);

        // Assert
        _mockOrderRepository.Verify();
        _mockGameService.Verify();
    }

    /// <summary>
    /// Test for UpdateOrderAsync method when a valid order is passed.
    /// Expects the order to be updated successfully.
    /// </summary>
    [Fact]
    public async Task UpdateOrderAsync_ValidOrder_UpdatesOrder()
    {
        // Arrange
        var testOrder = new OrderRequest
        {
            Id = "1",
            CustomerId = "123",
            ProductId = 1,
            ProductName = "Test Product",
            Quantity = 1,
            Sum = 10,
            OrderDate = DateTime.Now,
            Price = 10,
            Discount = 0,
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(Convert.ToInt32(testOrder.Id)))
            .ReturnsAsync(new Order { Id = Convert.ToInt32(testOrder.Id) })
            .Verifiable();

        _mockOrderRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _orderService.UpdateOrderAsync(testOrder);

        // Assert
        _mockOrderRepository.Verify();
    }

    /// <summary>
    /// Test for UpdateOrderAsync method when an order id that does not exist in the repository is passed.
    /// Expects a KeyNotFoundException to be thrown.
    /// </summary>
    [Fact]
    public async Task UpdateOrderAsync_OrderDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var testOrder = new OrderRequest { Id = "1" };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(Convert.ToInt32(testOrder.Id)))
            .ReturnsAsync(default(Order)); // no order found

        // Act and Assert
        var exception = await Record.ExceptionAsync(() => _orderService.UpdateOrderAsync(testOrder));

        Assert.NotNull(exception);
        Assert.IsType<KeyNotFoundException>(exception);
    }

    /// <summary>
    /// Test for RemoveOrderAsync method when a valid orderId is passed.
    /// Expects to call the repository's RemoveAsync method without any exception.
    /// </summary>
    [Fact]
    public async Task RemoveOrderAsync_ValidOrderId_RemovesOrder()
    {
        // Arrange
        int orderId = 1;

        _mockOrderRepository
            .Setup(r => r.RemoveAsync(orderId))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _orderService.RemoveOrderAsync(orderId);

        // Assert
        _mockOrderRepository.Verify();
    }

    /// <summary>
    /// Test for GetAllOrdersAsync method.
    /// Expects to return a list of OrderResponse objects.
    /// </summary>
    [Fact]
    public async Task GetAllOrdersAsync_ReturnsListOfOrderResponses()
    {
        // Arrange
        _mockOrderRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Order> { new() })
            .Verifiable();

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<OrderResponse>>(result);
        _mockOrderRepository.Verify();
    }

    /// <summary>
    /// Test AddOrderWithDetails method when no open order exists.
    /// The method is expected to create a new order and return OrderBuyResponse.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_NoExistingOpenOrder_CreatesNewOrder()
    {
        // Arrange
        string gameAlias = "game1";

        // simulate no existing open order
        _mockOrderRepository.Setup(repo => repo.GetFirstOpenOrderAsync())
            .ReturnsAsync((Order)null);

        var game = new Game { GameAlias = gameAlias, UnitInStock = 1 };
        _mockGameService.Setup(service => service.GetGameByAliasAsync(gameAlias))
            .ReturnsAsync(game);

        // Act
        var result = await _orderService.AddOrderWithDetails(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OrderBuyResponse>(result);
    }

    /// <summary>
    /// Test AddOrderWithDetails method when an open order exists.
    /// The method is expected to add order details to existing open order and return OrderBuyResponse.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_ExistingOpenOrder_AddsOrderDetailsToUpdate()
    {
        // Arrange
        string gameAlias = "game2";

        var openedOrder = new Order { OrderDetails = new List<OrderDetails>() };
        _mockOrderRepository.Setup(repo => repo.GetFirstOpenOrderAsync())
            .ReturnsAsync(openedOrder);

        var game = new Game { GameAlias = gameAlias, UnitInStock = 1 };
        _mockGameService.Setup(service => service.GetGameByAliasAsync(gameAlias))
            .ReturnsAsync(game);

        // Act
        var result = await _orderService.AddOrderWithDetails(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OrderBuyResponse>(result);
    }

    /// <summary>
    /// Test for AddOrderWithDetails method when an empty gameAlias is passed.
    /// It is expected to throw an ArgumentException.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_EmptyGameAlias_ThrowsArgumentException()
    {
        // Arrange
        string gameAlias = string.Empty;

        // Act and Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _orderService.AddOrderWithDetails(gameAlias));
        Assert.Equal("Game alias can't be null or empty. (Parameter 'gameAlias')", ex.Message);
    }

    /// <summary>
    /// Test for GetAllPaymentMethodsWithOrder method.
    /// Expects to return CreateOrderDTO object.
    /// </summary>
    [Fact]
    public async Task GetAllPaymentMethodsWithOrder_ReturnsCreateOrderDTO()
    {
        // Arrange
        _mockOrderRepository
            .Setup(repo => repo.GetFirstOpenOrderAsync())
            .ReturnsAsync(new Order());

        // Act
        var result = await _orderService.GetAllPaymentMethodsWithOrder();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreateOrderDTO>(result);
    }

    /// <summary>
    /// Test for RemoveOrderDetailsAsync method when valid gameAlias is passed.
    /// Expects repository's RemoveOrderDetailsAsync to be called without throwing any exceptions.
    /// </summary>
    [Fact]
    public async Task RemoveOrderDetailsAsync_ValidGameAlias_RemovesOrderDetails()
    {
        // Arrange
        string gameAlias = "game1";
        int orderId = 1;
        Game game = new() { GameAlias = gameAlias, UnitInStock = 5 };

        var orderDetailsList = new List<OrderDetails>
        {
            new()
            {
                Game = game,
                Quantity = 1,
            },
        };

        _mockOrderRepository.Setup(repo => repo.GetFirstOpenOrderAsync()).ReturnsAsync(new Order { Id = orderId });
        _mockOrderRepository.Setup(repo => repo.GetAllOrderDetails(orderId)).ReturnsAsync(orderDetailsList);
        _mockOrderRepository.Setup(repo => repo.RemoveOrderDetailsAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        _mockGameService.Setup(service => service.GetGameByAliasAsync(gameAlias)).ReturnsAsync(game);
        _mockGameService.Setup(service => service.UpdateGameWithoutDependenciesAsync(It.IsAny<Game>()))
            .Returns(Task.CompletedTask);

        // Act
        await _orderService.RemoveOrderDetailsAsync(gameAlias);

        // Assert
        _mockOrderRepository.Verify(repo => repo.GetFirstOpenOrderAsync(), Times.Once);
        _mockOrderRepository.Verify(repo => repo.GetAllOrderDetails(orderId), Times.Once);
        _mockOrderRepository.Verify(repo => repo.RemoveOrderDetailsAsync(It.IsAny<int>()), Times.Once);
        _mockGameService.Verify(service => service.GetGameByAliasAsync(gameAlias), Times.Once);
        _mockGameService.Verify(service => service.UpdateGameWithoutDependenciesAsync(It.IsAny<Game>()), Times.Once);
    }

    /// <summary>
    /// Test for GetOpenOrderDetailsAsync method.
    /// Should return a collection of CartDetailsDTO.
    /// </summary>
    [Fact]
    public async Task GetOpenOrderDetailsAsync_ReturnsOrderDetailsDTOs()
    {
        // Arrange
        var orderDetailsList = new List<OrderDetails>
        {
            new()
            {
                Game = new()
                {
                    GameAlias = "game1",
                    Name = "Test Game",
                    Price = 100,
                    UnitInStock = 5,
                    Discount = 10,
                },
            },
        };

        _mockOrderRepository
            .Setup(repo => repo.GetFirstOpenOrderAsync())
            .ReturnsAsync(new Order());

        _mockOrderRepository
            .Setup(repo => repo.GetAllOrderDetails(It.IsAny<int>()))
            .ReturnsAsync(orderDetailsList);

        // Act
        var result = await _orderService.GetOpenOrderDetailsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<CartDetailsDTO>>(result);
    }

    /// <summary>
    /// Test for GetBankPDFAsync method. Ensures it returns a valid byte array for a PDF.
    /// </summary>
    [Fact]
    public async Task GetBankPDFAsync_ValidPaymentRequest_ReturnsPDF()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDTO();
        var testOrder = new Order { Id = 1, Customer = new Customer { Id = 1 } };
        var testOrderDetails = new List<OrderDetails> { new() { Quantity = 1, Price = 1, Discount = 0 } };
        testOrder.OrderDetails = testOrderDetails;

        _mockOrderRepository.Setup(r => r.GetFirstOpenOrderAsync()).ReturnsAsync(testOrder);
        _mockOrderRepository.Setup(r => r.GetByIdWithOrderDetailsAsync(testOrder.Id)).ReturnsAsync(testOrder);
        _mockOrderRepository.Setup(r => r.CompleteOrder()).Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.GetBankPDFAsync(paymentRequest);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// Test for GetCartDetailsAsync method. Ensures it returns valid cart details for a valid order ID.
    /// </summary>
    [Fact]
    public async Task GetCartDetailsAsync_ValidOrderId_ReturnsCartDetails()
    {
        // Arrange
        int orderId = 1;
        var orderDetailsList = new List<OrderDetails> { new() { Quantity = 1, Game = new Game() } };

        _mockOrderRepository.Setup(repo => repo.GetAllOrderDetails(orderId)).ReturnsAsync(orderDetailsList);

        // Act
        var result = await _orderService.GetCartDetailsAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Test for GetIBoxTerminalOrderDetailsAsync method. Ensures it returns a valid IBoxResponseDTO object.
    /// </summary>
    [Fact]
    public async Task GetIBoxTerminalOrderDetailsAsync_ValidCall_ReturnIBoxResponse()
    {
        // Arrange
        var iboxResponse = new IBoxResponseDTO();

        _mockPaymentService.Setup(s => s.ProcessIboxPayment()).ReturnsAsync(iboxResponse);

        // Act
        var result = await _orderService.GetIBoxTerminalOrderDetailsAsync();

        // Assert
        Assert.Equal(iboxResponse, result);
    }

    /// <summary>
    /// Test for GetVisaOrderDetailsAsync method. Ensures it returns a valid IBoxResponseDTO object.
    /// </summary>
    [Fact]
    public async Task GetVisaOrderDetailsAsync_ValidCall_ReturnVisaResponse()
    {
        // Arrange
        var visaTransaction = new VisaTransactionDTO();
        var iboxResponse = new IBoxResponseDTO();

        _mockPaymentService.Setup(s => s.ProcessVisaPayment(visaTransaction)).ReturnsAsync(iboxResponse);

        // Act
        var result = await _orderService.GetVisaOrderDetailsAsync(visaTransaction);

        // Assert
        Assert.Equal(iboxResponse, result);
    }

    /// <summary>
    /// Tests whether AddOrderWithDetails throws an exception when the game is out of stock.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_NoStock_ThrowsException()
    {
        // Arrange
        string gameAlias = "game1";
        var game = new Game { GameAlias = gameAlias, UnitInStock = 0 };
        _mockGameService.Setup(s => s.GetGameByAliasAsync(gameAlias)).ReturnsAsync(game);

        // Act and Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.AddOrderWithDetails(gameAlias));
    }

    /// <summary>
    /// Tests whether AddOrderWithDetails updates the quantity of the existing order details.
    /// The test is passed if the quantity of the existing order details is incremented correctly.
    /// </summary>
    [Fact]
    public async Task AddOrderWithDetails_ExistingOrderDetails_UpdatesQuantity()
    {
        // Arrange
        string gameAlias = "game1";
        var game = new Game { GameAlias = gameAlias, UnitInStock = 5 };
        _mockGameService.Setup(s => s.GetGameByAliasAsync(gameAlias)).ReturnsAsync(game);

        List<OrderDetails> orderDetailsList = new();
        var existingOrderDetails = new OrderDetails { Quantity = 1, Game = game };
        orderDetailsList.Add(existingOrderDetails);

        var openedOrder = new Order { Status = OrderStatus.Open, OrderDetails = orderDetailsList, Id = 1 };

        _mockOrderRepository.Setup(s => s.GetFirstOpenOrderAsync()).ReturnsAsync(openedOrder);
        _mockOrderRepository.Setup(s => s.GetByIdWithOrderDetailsAsync(openedOrder.Id)).ReturnsAsync(openedOrder);

        // Act
        var response = await _orderService.AddOrderWithDetails(gameAlias);

        // Assert
        Assert.Equal(2, existingOrderDetails.Quantity);
    }
}
