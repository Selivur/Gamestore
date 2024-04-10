using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents a test class for the SQLProductRepository.
/// </summary>
public class SQLProductRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly SQLProductRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductRepositoryTests"/> class.
    /// </summary>
    public SQLProductRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GamestoreContext(_options);
        _repository = new SQLProductRepository(_context);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify that the GetAllProductsAsync method of the SQLProductRepository
    /// returns all products from the database.
    /// </summary>
    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Act
        var result = (await _repository.GetAllProductsAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// Test to verify that the GetProductByNameAsync method of the SQLProductRepository
    /// returns a product with the specified name when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetProductByNameAsync_ShouldReturnProduct()
    {
        // Act
        var result = await _repository.GetProductByNameAsync("Product 1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product 1", result.ProductName);
    }

    /// <summary>
    /// Test to verify that the GetProductByNameAsync method of the SQLProductRepository
    /// returns null when a product with the specified name does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetProductByNameAsync_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetProductByNameAsync("Non-Existent Product");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the AddProductAsync method of the SQLProductRepository adds a product to the database.
    /// </summary>
    [Fact]
    public async Task AddProductAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var product = new Product { ProductName = "New Product", QuantityPerUnit = "1" };

        // Act
        await _repository.AddProductAsync(product);

        // Assert
        var savedProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "New Product");
        Assert.NotNull(savedProduct);
        Assert.Equal(product.ProductName, savedProduct.ProductName);
    }

    /// <summary>
    /// Test to verify that the UpdateProductAsync method of the SQLProductRepository updates a product in the database.
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var product = await _context.Products.SingleAsync(g => g.ProductID == 1);
        product.ProductName = "Updated Product";

        // Act
        await _repository.UpdateProductAsync(product);

        // Assert
        var updatedProduct = await _context.Products.FirstOrDefaultAsync(g => g.ProductID == 1);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct.ProductName);
    }

    /// <summary>
    /// Test to verify that the DeleteProductAsync method of the SQLProductRepository removes a product from the database.
    /// </summary>
    [Fact]
    public async Task DeleteProductAsync_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        var productId = 2;

        // Act
        await _repository.DeleteProductAsync(productId);

        // Assert
        var removedProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        Assert.Null(removedProduct);
    }

    /// <summary>
    /// Test to verify that the DeleteProductAsync method of the SQLProductRepository does not throw an exception when the product does not exist in the database.
    /// </summary>
    [Fact]
    public async Task DeleteProductAsync_ShouldNotThrowExceptionWhenProductNotFound()
    {
        // Act & Assert
        await _repository.DeleteProductAsync(999);
    }

    /// <summary>
    /// Cleans up the resources after each test by deleting the database.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Seeds the database with initial data for testing.
    /// </summary>
    private void InitializeTestData()
    {
        using var context = new GamestoreContext(_options);

        var products = new List<Product>
        {
            new() { ProductID = 1, ProductName = "Product 1", QuantityPerUnit = "1" },
            new() { ProductID = 2, ProductName = "Product 2", QuantityPerUnit = "1" },
            new() { ProductID = 3, ProductName = "Product 3", QuantityPerUnit = "1" },
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}