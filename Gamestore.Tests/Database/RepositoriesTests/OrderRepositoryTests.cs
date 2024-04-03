using Gamestore.Database.Entities.Enums;
using Microsoft.Extensions.Options;

namespace Gamestore.Tests.Database.RepositoriesTests;
public class OrderRepositoryTests : IDisposable
{
    private readonly OrderRepository _repository;
    private readonly GamestoreContext _context;
    private readonly MongoContext _mongoContext;

    private Order _testOrder;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new GamestoreContext(options);

        var mongoSettings = Options.Create(new MongoSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "Northwind",
        });
        _mongoContext = new MongoContext(mongoSettings);
        _repository = new OrderRepository(_context, _mongoContext);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify if GetByIdAsync method returns the order with the corresponding Id when a valid Id is provided.
    /// The data context is initialized with test data.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsOrder()
    {
        // Act
        var fetchedOrder = await _repository.GetByIdAsync(_testOrder.Id);

        // Assert
        Assert.NotNull(fetchedOrder);
        Assert.Equal(_testOrder.Id, fetchedOrder.Id);
    }

    /// <summary>
    /// Test to verify if GetFirstOpenOrderAsync method returns the first open order.
    /// The data context is initialized with test data.
    /// </summary>
    [Fact]
    public async Task GetFirstOpenOrderAsync_WhenCalled_ReturnsFirstOpenOrder()
    {
        // Act
        var fetchedOrder = await _repository.GetFirstOpenOrderAsync();

        // Assert
        Assert.NotNull(fetchedOrder);
        Assert.Equal(_testOrder.Id, fetchedOrder.Id);
    }

    /// <summary>
    /// Test to verify if GetByIdWithOrderDetailsAsync method returns the order with corresponding order details when a valid Id is provided.
    /// The data context is initialized with test data.
    /// </summary>
    [Fact]
    public async Task GetByIdWithOrderDetailsAsync_ValidId_ReturnsOrderWithOrderDetails()
    {
        // Act
        var fetchedOrder = await _repository.GetByIdWithOrderDetailsAsync(_testOrder.Id);

        // Assert
        Assert.NotNull(fetchedOrder);
        Assert.Equal(_testOrder.OrderDetails.Count, fetchedOrder.OrderDetails.Count);
    }

    /// <summary>
    /// Test to verify if GetAllAsync method returns all orders.
    /// The data context is initialized with test data.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsAllOrders()
    {
        // Act
        var fetchedOrders = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(fetchedOrders);
        Assert.Single(fetchedOrders); // We only inserted one order in InitializeTestData
    }

    /// <summary>
    /// Test to verify if AddAsync method adds a new order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task AddAsync_ValidOrder_AddsOrder()
    {
        // Arrange
        var newOrder = new Order { Id = 5, Customer = new Customer { Name = "Customer Test" }, OrderDetails = new List<OrderDetails>() { new() } };

        // Act
        await _repository.AddAsync(newOrder);

        // Assert
        Assert.NotNull(await _context.Orders.FindAsync(newOrder.Id));
    }

    /// <summary>
    /// Test to verify if UpdateAsync method updates an existing order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ValidOrder_UpdatesOrder()
    {
        // Arrange
        _testOrder.Status = OrderStatus.Paid;

        // Act
        await _repository.UpdateAsync(_testOrder);

        // Assert
        var updatedOrder = await _context.Orders.FindAsync(_testOrder.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Paid, updatedOrder.Status);
    }

    /// <summary>
    /// Test to verify if UpdateOrderDetailsAsync method updates an existing order detail.
    /// The data context is initialized with test data.
    /// </summary>
    [Fact]
    public async Task UpdateOrderDetailsAsync_ValidOrderDetails_UpdatesOrderDetails()
    {
        // Arrange
        _testOrder.OrderDetails.First().Quantity = 5;

        // Act
        await _repository.UpdateOrderDetailsAsync(_testOrder.OrderDetails.First());

        // Assert
        var updatedOrderDetails = await _context.OrderDetails.FindAsync(_testOrder.OrderDetails.First().Id);
        Assert.NotNull(updatedOrderDetails);
        Assert.Equal(5, updatedOrderDetails.Quantity);
    }

    /// <summary>
    /// Test to verify if RemoveAsync method deletes an existing order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ValidOrderId_RemovesOrder()
    {
        // Act
        await _repository.RemoveAsync(1);

        // Assert
        Assert.Null(await _context.Orders.FindAsync(1));
    }

    /// <summary>
    /// Test to verify if RemoveAsync method throws ArgumentException when no order is found with the provided id.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_InvalidOrderId_ThrowsArgumentException()
    {
        // Arrange
        var nonExistantOrderId = 999;

        // Act and Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveAsync(nonExistantOrderId));
        Assert.Contains($"No order found with the id '{nonExistantOrderId}'.", ex.Message);
    }

    /// <summary>
    /// Test to verify if RemoveOrderDetailsAsync method deletes existing order details.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task RemoveOrderDetailsAsync_ValidOrderDetailsId_RemovesOrderDetails()
    {
        // Arrange
        var testOrderDetailsId = _testOrder.OrderDetails.First().Id;

        // Act
        await _repository.RemoveOrderDetailsAsync(testOrderDetailsId);

        // Assert
        Assert.Null(await _context.OrderDetails.FindAsync(testOrderDetailsId));
    }

    /// <summary>
    /// Test to verify if RemoveOrderDetailsAsync method throws ArgumentException when no order details is found.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task RemoveOrderDetailsAsync_InvalidOrderDetailsId_ThrowsArgumentException()
    {
        // Arrange
        var nonExistantOrderDetailsId = 999;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveOrderDetailsAsync(nonExistantOrderDetailsId));
        Assert.Contains($"No order found with the id '{nonExistantOrderDetailsId}'.", ex.Message);
    }

    /// <summary>
    /// Test to verify if GetAllOrderDetails method returns all order details for a particular order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task GetAllOrderDetails_ValidOrderId_ReturnsAllOrderDetailsForTheOrder()
    {
        // Arrange
        var testOrderId = _testOrder.Id;

        // Act
        var fetchedOrderDetails = await _repository.GetAllOrderDetails(testOrderId);

        // Assert
        Assert.NotNull(fetchedOrderDetails);
        Assert.Single(fetchedOrderDetails);
    }

    /// <summary>
    /// Test to verify if CompleteOrder method updates the status of the first open order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task CompleteOrder_WhenCalled_ChangesOrderStatusToPaid()
    {
        // Act
        await _repository.CompleteOrder();

        // Assert
        var firstOpenOrderInDb = await _context.Orders.Where(o => o.Id == _testOrder.Id).FirstOrDefaultAsync();
        Assert.NotNull(firstOpenOrderInDb);
        Assert.Equal(OrderStatus.Paid, firstOpenOrderInDb.Status);
    }

    /// <summary>
    /// Test to verify if CancelledOrder method updates the status of the first open order.
    /// The test uses an in-memory database to simulate the data context.
    /// </summary>
    [Fact]
    public async Task CancelledOrder_WhenCalled_ChangesOrderStatusToCancelled()
    {
        // Act
        await _repository.CancelledOrder();

        // Assert
        var firstOpenOrderInDb = await _context.Orders.Where(o => o.Id == _testOrder.Id).FirstOrDefaultAsync();
        Assert.NotNull(firstOpenOrderInDb);
        Assert.Equal(OrderStatus.Cancelled, firstOpenOrderInDb.Status);
    }

    /// <summary>
    /// Clean up the resources used by the class.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    private void InitializeTestData()
    {
        _testOrder = new Order
        {
            Id = 1,
            Status = OrderStatus.Open,
            Customer = new Customer { Id = 1, Name = "Customer1" },
            OrderDetails = new List<OrderDetails>
            {
                new()
                {
                    Id = 1,
                    Game = new Game
                    {
                        Id = 1,
                        Name = "TestGame1",
                        GameAlias = "tg1",
                    },
                    Quantity = 2,
                },
            },
        };

        _context.Orders.Add(_testOrder);
        _context.SaveChanges();
    }
}
