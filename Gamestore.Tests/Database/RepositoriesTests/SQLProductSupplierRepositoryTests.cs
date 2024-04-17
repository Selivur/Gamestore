using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Services.Interfaces;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents a test class for the SQLProductSupplierRepository.
/// </summary>
public class SQLProductSupplierRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly SQLProductSupplierRepository _repository;
    private readonly Mock<IDataBaseLogger> _mockLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductSupplierRepositoryTests"/> class.
    /// </summary>
    public SQLProductSupplierRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockLogger = new Mock<IDataBaseLogger>();

        _context = new GamestoreContext(_options);
        _repository = new SQLProductSupplierRepository(_context, _mockLogger.Object);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify that the GetAllProductSuppliersAsync method of the SQLProductSupplierRepository
    /// returns all product suppliers from the database.
    /// </summary>
    [Fact]
    public async Task GetAllProductSuppliersAsync_ShouldReturnAllProductSuppliers()
    {
        // Act
        var result = (await _repository.GetAllProductSuppliersAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// Test to verify that the GetProductSupplierByCompanyNameAsync method of the SQLProductSupplierRepository
    /// returns a product supplier with the specified company name when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetProductSupplierByCompanyNameAsync_ShouldReturnProductSupplier()
    {
        // Act
        var result = await _repository.GetProductSupplierByCompanyNameAsync("Company 1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Company 1", result.CompanyName);
    }

    /// <summary>
    /// Test to verify that the GetProductSupplierByCompanyNameAsync method of the SQLProductSupplierRepository
    /// throws an exception when a product supplier with the specified company name does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetProductSupplierByCompanyNameAsync_ShouldThrowExceptionWhenProductSupplierNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.GetProductSupplierByCompanyNameAsync("Non-Existent Company"));
    }

    /// <summary>
    /// Test to verify that the AddProductSupplierAsync method of the SQLProductSupplierRepository adds a product supplier to the database.
    /// </summary>
    [Fact]
    public async Task AddProductSupplierAsync_ShouldAddProductSupplierToDatabase()
    {
        // Arrange
        var productSupplier = new ProductSupplier
        {
            CompanyName = "New Company",
            Address = "123 Main St",
            City = "City",
            ContactName = "John Doe",
            ContactTitle = "Manager",
            Country = "Country",
            Fax = "123-456-7890",
            HomePage = "www.example.com",
            Phone = "555-1234",
            PostalCode = "12345",
            Region = "Region",
        };

        // Act
        await _repository.AddProductSupplierAsync(productSupplier);

        // Assert
        var savedProductSupplier = await _context.ProductSuppliers.FirstOrDefaultAsync(ps => ps.CompanyName == "New Company");
        Assert.NotNull(savedProductSupplier);
        Assert.Equal(productSupplier.CompanyName, savedProductSupplier.CompanyName);
    }

    /// <summary>
    /// Test to verify that the UpdateProductSupplierAsync method of the SQLProductSupplierRepository updates a product supplier in the database.
    /// </summary>
    [Fact]
    public async Task UpdateProductSupplierAsync_ShouldUpdateProductSupplierInDatabase()
    {
        // Arrange
        var productSupplier = await _context.ProductSuppliers.SingleAsync(ps => ps.CompanyName == "Company 1");
        productSupplier.CompanyName = "Updated Company";

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateProductSupplierAsync(productSupplier);

        // Assert
        var updatedProductSupplier = await _context.ProductSuppliers.FirstOrDefaultAsync(ps => ps.CompanyName == "Updated Company");
        Assert.NotNull(updatedProductSupplier);
        Assert.Equal("Updated Company", updatedProductSupplier.CompanyName);
    }

    /// <summary>
    /// Test to verify that the DeleteProductSupplierAsync method of the SQLProductSupplierRepository removes a product supplier from the database.
    /// </summary>
    [Fact]
    public async Task DeleteProductSupplierAsync_ShouldRemoveProductSupplierFromDatabase()
    {
        // Arrange
        var productSupplierId = 1;

        // Act
        await _repository.DeleteProductSupplierAsync(productSupplierId);

        // Assert
        var removedProductSupplier = await _context.ProductSuppliers.FirstOrDefaultAsync(ps => ps.SupplierId == productSupplierId);
        Assert.Null(removedProductSupplier);
    }

    /// <summary>
    /// Test to verify that the DeleteProductSupplierAsync method of the SQLProductSupplierRepository does not throw an exception when the product supplier does not exist in the database.
    /// </summary>
    [Fact]
    public async Task DeleteProductSupplierAsync_ShouldNotThrowExceptionWhenProductSupplierNotFound()
    {
        // Act & Assert
        await _repository.DeleteProductSupplierAsync(999);
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

        var productSuppliers = new List<ProductSupplier>
        {
            new()
            {
                SupplierId = 1,
                CompanyName = "Company 1",
                ContactName = "John Doe",
                ContactTitle = "Manager",
                Address = "123 Main St",
                City = "City",
                Region = "Region",
                PostalCode = "12345",
                Country = "Country",
                Phone = "555-1234",
                Fax = "123-456-7890",
                HomePage = "www.example.com",
            },
            new()
            {
                SupplierId = 2,
                CompanyName = "Company 2",
                ContactName = "Jane Smith",
                ContactTitle = "Sales Representative",
                Address = "456 Elm St",
                City = "City",
                Region = "Region",
                PostalCode = "54321",
                Country = "Country",
                Phone = "555-5678",
                Fax = "987-654-3210",
                HomePage = "www.example2.com",
            },
            new()
            {
                SupplierId = 3,
                CompanyName = "Company 3",
                ContactName = "Bob Johnson",
                ContactTitle = "Account Manager",
                Address = "789 Oak St",
                City = "City",
                Region = "Region",
                PostalCode = "67890",
                Country = "Country",
                Phone = "555-9012",
                Fax = "210-987-6543",
                HomePage = "www.example3.com",
            },
        };

        context.ProductSuppliers.AddRange(productSuppliers);
        context.SaveChanges();
    }
}