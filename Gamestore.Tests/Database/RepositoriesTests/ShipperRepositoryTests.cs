using Gamestore.Database.Entities.MongoDB;
using MongoDB.Driver;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents set of unit tests for ShipperRepository.
/// </summary>
public class ShipperRepositoryTests
{
    private readonly Mock<IMongoCollection<ProductShipper>> _mockCollection;
    private readonly ShipperRepository _repository;
    private readonly List<ProductShipper> _shippers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperRepositoryTests"/> class.
    /// </summary>
    public ShipperRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<ProductShipper>>();
        _repository = new ShipperRepository(_mockCollection.Object);

        _shippers = new List<ProductShipper>
            {
                new() { ShipperId = 1, CompanyName = "Shipper1", Phone = "123" },
                new() { ShipperId = 2, CompanyName = "Shipper2", Phone = "456" },
            };
    }

    /// <summary>
    /// Test GetAllShippersAsync method returns all shippers.
    /// </summary>
    [Fact]
    public async Task GetAllShippersAsync_ReturnsAllShippers()
    {
        var cursorMock = new Mock<IAsyncCursor<ProductShipper>>();
        cursorMock.Setup(cm => cm.Current).Returns(_shippers);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        _mockCollection.Setup(m => m.FindAsync(
            It.IsAny<FilterDefinition<ProductShipper>>(),
            It.IsAny<FindOptions<ProductShipper, ProductShipper>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetAllShippersAsync();

        Assert.Equal(_shippers.Count, result.Count);
    }
}