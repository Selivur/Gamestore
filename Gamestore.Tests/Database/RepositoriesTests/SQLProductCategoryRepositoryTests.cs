using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Services.Interfaces;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents a test class for the SQLProductCategoryRepository.
/// </summary>
public class SQLProductCategoryRepositoryTests : IDisposable
{
    private readonly Mock<IDataBaseLogger> _mockLogger;
    private readonly GamestoreContext _fakeContext;
    private readonly SQLProductCategoryRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductCategoryRepositoryTests"/> class.
    /// </summary>
    public SQLProductCategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GamestoreContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        _fakeContext = new GamestoreContext(options);

        _fakeContext.ProductCategories.AddRange(new List<ProductCategory>
        {
            new() { CategoryID = 1, CategoryName = "Test1", Description = "Test Description 1", Picture = "Test Picture 1" },
            new() { CategoryID = 2, CategoryName = "Test2", Description = "Test Description 2", Picture = "Test Picture 2" },
        });

        _fakeContext.SaveChanges();

        _mockLogger = new Mock<IDataBaseLogger>();

        _repository = new SQLProductCategoryRepository(_fakeContext, _mockLogger.Object);
    }

    /// <summary>
    /// Tests whether GetAllProductCategoriesAsync method returns all product categories from the test database.
    /// </summary>
    [Fact]
    public async Task GetAllProductCategoriesAsync_ReturnsAllCategories()
    {
        // Act
        var result = await _repository.GetAllProductCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    /// <summary>
    /// Tests GetProductCategoryByIdAsync method with a valid ID parameter.
    /// The returned category is expected to have the same ID.
    /// </summary>
    [Fact]
    public async Task GetProductCategoryByIdAsync_ValidId_ReturnsCategory()
    {
        // Act
        var result = await _repository.GetProductCategoryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CategoryID);
        Assert.Equal("Test1", result.CategoryName);
    }

    /// <summary>
    /// Tests GetProductCategoryByIdAsync method with an invalid ID parameter.
    /// An invalid ID should return null.
    /// </summary>
    [Fact]
    public async Task GetProductCategoryByIdAsync_InvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetProductCategoryByIdAsync(3);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests AddProductCategoryAsync method with a valid ProductCategory object.
    /// The category is expected to be added to the test database.
    /// </summary>
    [Fact]
    public async Task AddProductCategoryAsync_ValidCategory_AddsCategory()
    {
        // Arrange
        var newCategory = new ProductCategory { CategoryID = 3, CategoryName = "Test3", Description = "Test Description 3", Picture = "Test Picture 3" };

        // Act
        await _repository.AddProductCategoryAsync(newCategory);

        // Assert
        var savedCategory = await _fakeContext.ProductCategories.FindAsync(3);
        Assert.NotNull(savedCategory);
        Assert.Equal("Test3", savedCategory.CategoryName);
    }

    /// <summary>
    /// Tests UpdateProductCategoryAsync method with a valid ProductCategory object.
    /// The category is expected to be updated in the test database.
    /// </summary>
    [Fact]
    public async Task UpdateProductCategoryAsync_ValidCategory_UpdatesCategory()
    {
        // Arrange
        var category = await _fakeContext.ProductCategories.FindAsync(2);
        category.CategoryName = "Updated";

        // Act
        await _repository.UpdateProductCategoryAsync(category);

        // Assert
        var savedCategory = await _fakeContext.ProductCategories.FindAsync(2);
        Assert.Equal("Updated", savedCategory.CategoryName);
    }

    /// <summary>
    /// Tests DeleteProductCategoryAsync method with a valid ID parameter.
    /// The category with the given ID is expected to be removed from the test database.
    /// </summary>
    [Fact]
    public async Task DeleteProductCategoryAsync_ValidId_DeletesCategory()
    {
        // Act
        await _repository.DeleteProductCategoryAsync(2);

        // Assert
        var deletedCategory = await _fakeContext.ProductCategories.FindAsync(2);
        Assert.Null(deletedCategory);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fakeContext.Dispose();
        }
    }
}
