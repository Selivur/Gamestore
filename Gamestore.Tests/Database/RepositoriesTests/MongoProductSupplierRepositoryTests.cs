using Gamestore.Database.Entities.MongoDB;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Class provides unit tests for methods in the MongoProductSupplierRepository.
/// </summary>
public class MongoProductSupplierRepositoryTests
{
    private readonly Mock<IMongoCollection<ProductSupplier>> _mockCollection;
    private readonly MongoProductSupplierRepository _repository;
    private readonly List<ProductSupplier> _productSuppliers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductSupplierRepositoryTests"/> class.
    /// </summary>
    public MongoProductSupplierRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<ProductSupplier>>();
        _repository = new MongoProductSupplierRepository(_mockCollection.Object);

        _productSuppliers = new List<ProductSupplier>
        {
            new() { SupplierId = 1, CompanyName = "Supplier1" },
            new() { SupplierId = 2, CompanyName = "Supplier2" },
        };
    }

    /// <summary>
    /// Test for the GetAllProductSuppliersAsync method. Should return all product suppliers.
    /// </summary>
    [Fact]
    public async Task GetAllProductSuppliersAsync_ReturnsAllProductSuppliers()
    {
        var cursorMock = new Mock<IAsyncCursor<ProductSupplier>>();
        cursorMock.Setup(cm => cm.Current).Returns(_productSuppliers);
        cursorMock.SetReturnsDefault(true);
        cursorMock.SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        _mockCollection.Setup(m => m.FindAsync(It.IsAny<FilterDefinition<ProductSupplier>>(), It.IsAny<FindOptions<ProductSupplier, ProductSupplier>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetAllProductSuppliersAsync();

        Assert.Equal(_productSuppliers.Count, result.Count());
    }

    /// <summary>
    /// Test GetProductSupplierByCompanyNameAsync method returns the correct product supplier.
    /// </summary>
    [Fact]
    public async Task GetProductSupplierByCompanyNameAsync_ReturnsCorrectProductSupplier()
    {
        const string testCompanyName = "Supplier1";
        var testProductSupplier = new ProductSupplier() { SupplierId = 1, CompanyName = testCompanyName };

        var cursorMock = new Mock<IAsyncCursor<ProductSupplier>>();
        cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));
        cursorMock.Setup(cm => cm.Current).Returns(new[] { testProductSupplier });

        _mockCollection.Setup(m => m.FindAsync(
                It.IsAny<FilterDefinition<ProductSupplier>>(),
                It.IsAny<FindOptions<ProductSupplier, ProductSupplier>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetProductSupplierByCompanyNameAsync(testCompanyName);

        Assert.NotNull(result);
        Assert.Equal(testCompanyName, result.CompanyName);
    }

    /// <summary>
    /// Test AddProductSupplierAsync method adds a product supplier.
    /// </summary>
    [Fact]
    public async Task AddProductSupplierAsync_CallsInsertOneAsync()
    {
        var productSupplier = new ProductSupplier() { SupplierId = 3, CompanyName = "Supplier3" };

        _mockCollection.Setup(m => m.InsertOneAsync(
                It.Is<ProductSupplier>(p => p.SupplierId == productSupplier.SupplierId),
                null,
                default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _repository.AddProductSupplierAsync(productSupplier);

        _mockCollection.Verify();
    }

    /// <summary>
    /// Test UpdateProductSupplierAsync method updates a product supplier.
    /// </summary>
    [Fact]
    public async Task UpdateProductSupplierAsync_CallsReplaceOneAsync()
    {
        var productSupplier = new ProductSupplier() { SupplierId = 1, CompanyName = "UpdatedSupplier1" };

        _mockCollection
            .Setup(m => m.ReplaceOneAsync(
                It.IsAny<FilterDefinition<ProductSupplier>>(),
                It.IsAny<ProductSupplier>(),
                It.IsAny<ReplaceOptions>(),
                default))
            .Callback<FilterDefinition<ProductSupplier>, ProductSupplier, ReplaceOptions, CancellationToken>((filter, p, options, token) =>
            {
                var renderedFilter = filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<ProductSupplier>(), BsonSerializer.SerializerRegistry);
                var expectedFilter = Builders<ProductSupplier>.Filter.Eq(p => p.SupplierId, productSupplier.SupplierId).Render(BsonSerializer.SerializerRegistry.GetSerializer<ProductSupplier>(), BsonSerializer.SerializerRegistry);
                Assert.Equal(expectedFilter, renderedFilter);
                Assert.Equal(productSupplier.SupplierId, p.SupplierId);
            })
            .ReturnsAsync(It.IsAny<ReplaceOneResult>());

        await _repository.UpdateProductSupplierAsync(productSupplier);
    }

    /// <summary>
    /// Test DeleteProductSupplierAsync method deletes a product supplier.
    /// </summary>
    [Fact]
    public async Task DeleteProductSupplierAsync_CallsDeleteOneAsync()
    {
        var mockCollection = new Mock<IMongoCollection<ProductSupplier>>();

        int id = 1;
        var expectedFilter = Builders<ProductSupplier>.Filter.Eq(p => p.SupplierId, id);

        var repository = new MongoProductSupplierRepository(mockCollection.Object);

        mockCollection
            .Setup(x => x.DeleteOneAsync(
                It.IsAny<ExpressionFilterDefinition<ProductSupplier>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(1))
            .Verifiable();

        await repository.DeleteProductSupplierAsync(id);

        mockCollection.Verify(
            x => x.DeleteOneAsync(
                It.Is<FilterDefinition<ProductSupplier>>(fd => fd.Render(BsonSerializer.SerializerRegistry.GetSerializer<ProductSupplier>(), BsonSerializer.SerializerRegistry)
                    .ToString() == expectedFilter.Render(BsonSerializer.SerializerRegistry.GetSerializer<ProductSupplier>(), BsonSerializer.SerializerRegistry).ToString()),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
