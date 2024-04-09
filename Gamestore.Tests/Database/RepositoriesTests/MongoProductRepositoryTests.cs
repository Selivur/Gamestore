using Gamestore.Database.Entities.MongoDB;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Unit tests for the MongoProductRepository class.
/// </summary>
public class MongoProductRepositoryTests
{
    private readonly Mock<IMongoCollection<Product>> _mockCollection;
    private readonly MongoProductRepository _repository;
    private readonly List<Product> _products;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductRepositoryTests"/> class.
    /// </summary>
    public MongoProductRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<Product>>();
        _repository = new MongoProductRepository(_mockCollection.Object);

        _products = new List<Product>
            {
                new() { ProductID = 1, ProductName = "Product1", UnitPrice = 100 },
                new() { ProductID = 2, ProductName = "Product2", UnitPrice = 200 },
            };
    }

    /// <summary>
    /// Test for the GetAllProductsAsync method. Should return all products.
    /// </summary>
    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        var cursorMock = new Mock<IAsyncCursor<Product>>();
        cursorMock.Setup(cm => cm.Current).Returns(_products);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        _mockCollection.Setup(m => m.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetAllProductsAsync();

        Assert.Equal(_products.Count, result.Count());
    }

    /// <summary>
    /// Test GetProductByNameAsync method returns the correct product.
    /// </summary>
    [Fact]
    public async Task GetProductByNameAsync_ReturnsCorrectProduct()
    {
        const string testName = "Product1";
        var testProduct = new Product() { ProductID = 1, ProductName = testName, UnitPrice = 100 };

        var cursorMock = new Mock<IAsyncCursor<Product>>();
        cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));
        cursorMock.Setup(c => c.Current).Returns(new[] { testProduct });

        _mockCollection.Setup(m => m.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetProductByNameAsync(testName);

        Assert.NotNull(result);
        Assert.Equal(testName, result.ProductName);
    }

    /// <summary>
    /// Test AddProductAsync method adds a product.
    /// </summary>
    [Fact]
    public async Task AddProductAsync_CallsInsertOneAsync()
    {
        var product = new Product() { ProductID = 3, ProductName = "Product3", UnitPrice = 300 };

        _mockCollection.Setup(m => m.InsertOneAsync(
                It.Is<Product>(p => p.ProductID == product.ProductID),
                null,
                default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _repository.AddProductAsync(product);

        _mockCollection.Verify();
    }

    /// <summary>
    /// Test UpdateProductAsync method updates a product.
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_CallsReplaceOneAsync()
    {
        var product = new Product() { ProductID = 1, ProductName = "UpdatedProduct1", UnitPrice = 100 };

        _mockCollection
            .Setup(m => m.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                default))
            .Callback<FilterDefinition<Product>, Product, ReplaceOptions, CancellationToken>((filter, p, options, token) =>
            {
                var renderedFilter = filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Product>(), BsonSerializer.SerializerRegistry);
                var expectedFilter = Builders<Product>.Filter.Eq(p => p.ProductID, product.ProductID).Render(BsonSerializer.SerializerRegistry.GetSerializer<Product>(), BsonSerializer.SerializerRegistry);
                Assert.Equal(expectedFilter, renderedFilter);
                Assert.Equal(product.ProductID, p.ProductID);
            })
            .ReturnsAsync(It.IsAny<ReplaceOneResult>()); // Я використав це.

        await _repository.UpdateProductAsync(product);
    }

    /// <summary>
    /// Test DeleteProductAsync method deletes a product.
    /// </summary>
    [Fact]
    public async Task DeleteProductAsync_CallsDeleteOneAsync()
    {
        // Arrange
        var mockCollection = new Mock<IMongoCollection<Product>>();

        int id = 1;
        var expectedFilter = Builders<Product>.Filter.Eq(p => p.ProductID, id);

        var repository = new MongoProductRepository(mockCollection.Object);

        mockCollection
            .Setup(x => x.DeleteOneAsync(
                It.IsAny<ExpressionFilterDefinition<Product>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(1))
            .Verifiable();

        // Act
        await repository.DeleteProductAsync(id);

        // Assert
        mockCollection.Verify(
            x => x.DeleteOneAsync(
            It.Is<FilterDefinition<Product>>(fd => fd.Render(BsonSerializer.SerializerRegistry.GetSerializer<Product>(), BsonSerializer.SerializerRegistry).ToString() == expectedFilter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Product>(), BsonSerializer.SerializerRegistry).ToString()),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}