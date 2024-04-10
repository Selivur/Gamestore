using Gamestore.Database.Entities.MongoDB;
using MongoDB.Driver;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Unit tests for the Gamestore.Database.Repositories.MongoOrderRepository class.
/// </summary>
public class MongoOrderRepositoryTests
{
    private readonly Mock<IMongoCollection<ProductOrder>> _mockCollection;
    private readonly MongoOrderRepository _repository;
    private readonly List<ProductOrder> _orders;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoOrderRepositoryTests"/> class.
    /// </summary>
    public MongoOrderRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<ProductOrder>>();
        _repository = new MongoOrderRepository(_mockCollection.Object);

        _orders = new List<ProductOrder>
        {
            new()
            {
                OrderID = 1,
                CustomerId = "Customer1",
                EmployeeID = 1,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(7),
                ShippedDate = DateTime.Now.AddDays(2),
                ShipVia = 1,
                Freight = 10.5,
                ShipName = "Ship Name 1",
                ShipAddress = "Ship Address 1",
                ShipCity = "Ship City 1",
                ShipRegion = "Ship Region 1",
                ShipPostalCode = "12345",
                ShipCountry = "Ship Country 1",
            },
            new()
            {
                OrderID = 2,
                CustomerId = "Customer2",
                EmployeeID = 2,
                OrderDate = DateTime.Now.AddDays(-7),
                RequiredDate = DateTime.Now.AddDays(14),
                ShippedDate = null,
                ShipVia = 2,
                Freight = 15.75,
                ShipName = "Ship Name 2",
                ShipAddress = "Ship Address 2",
                ShipCity = "Ship City 2",
                ShipRegion = "Ship Region 2",
                ShipPostalCode = "54321",
                ShipCountry = "Ship Country 2",
            },
        };
    }

    /// <summary>
    /// Test GetAllAsync method returns all orders.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var cursorMock = new Mock<IAsyncCursor<ProductOrder>>();
        cursorMock.Setup(cm => cm.Current).Returns(_orders);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        _mockCollection.Setup(m => m.FindAsync(
            It.IsAny<FilterDefinition<ProductOrder>>(),
            It.IsAny<FindOptions<ProductOrder, ProductOrder>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetAllAsync();

        Assert.Equal(_orders.Count, result.Count());
    }
}
