using Gamestore.Database.Entities.MongoDB;
using MongoDB.Driver;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents set of unit tests for MongoProductCategoryRepository class.
/// </summary>
public class MongoProductCategoryRepositoryTests
{
    private readonly Mock<IMongoCollection<ProductCategory>> _mockCollection;
    private readonly MongoProductCategoryRepository _repository;
    private readonly List<ProductCategory> _categories;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductCategoryRepositoryTests"/> class.
    /// </summary>
    public MongoProductCategoryRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<ProductCategory>>();
        _repository = new MongoProductCategoryRepository(_mockCollection.Object);

        _categories = new List<ProductCategory>
            {
                new() { CategoryID = 1, CategoryName = "Category1" },
                new() { CategoryID = 2, CategoryName = "Category2" },
            };
    }

    /// <summary>
    /// Test GetAllProductCategoriesAsync method returns all categories.
    /// </summary>
    [Fact]
    public async Task GetAllProductCategoriesAsync_ReturnsAllCategories()
    {
        var cursorMock = new Mock<IAsyncCursor<ProductCategory>>();
        cursorMock.Setup(cm => cm.Current).Returns(_categories);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock
            .SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        _mockCollection.Setup(m => m.FindAsync(
            It.IsAny<FilterDefinition<ProductCategory>>(),
            It.IsAny<FindOptions<ProductCategory, ProductCategory>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetAllProductCategoriesAsync();

        Assert.Equal(_categories.Count, result.Count());
    }

    /// <summary>
    /// Test GetProductCategoryByIdAsync method returns correct category.
    /// </summary>
    [Fact]
    public async Task GetProductCategoryByIdAsync_ReturnsCorrectCategory()
    {
        const int testId = 1;
        var testCategories = new List<ProductCategory> { new() { CategoryID = testId, CategoryName = "TestCategory" } };

        var cursorMock = new Mock<IAsyncCursor<ProductCategory>>();
        cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock.SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));
        cursorMock.Setup(cm => cm.Current).Returns(testCategories);

        _mockCollection.Setup(m => m.FindAsync(It.IsAny<FilterDefinition<ProductCategory>>(), It.IsAny<FindOptions<ProductCategory, ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var result = await _repository.GetProductCategoryByIdAsync(testId);

        Assert.NotNull(result);
        Assert.Equal(testCategories.First().CategoryID, result.CategoryID);
        Assert.Equal(testCategories.First().CategoryName, result.CategoryName);
    }
}
