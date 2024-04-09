using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services;
using Moq;

namespace Gamestore.Tests.Database.ServicesTests;

/// <summary>
/// Class provides unit tests for methods in the ProductService.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockSqlRepository;
    private readonly Mock<IProductRepository> _mockMongoRepository;
    private readonly ProductService _productService;
    private readonly Product _testProduct;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductServiceTests"/> class.
    /// </summary>
    public ProductServiceTests()
    {
        _mockSqlRepository = new Mock<IProductRepository>();
        _mockMongoRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockSqlRepository.Object, _mockMongoRepository.Object);
        _testProduct = new Product { ProductID = 1, ProductName = "TestProduct", UnitPrice = 100 };
    }

    /// <summary>
    /// Test for the AddProductAsync method. It adds a new product into repositories.
    /// </summary>
    [Fact]
    public async Task AddProductAsync_CallsAddProductAsyncOnRepositories()
    {
        _mockSqlRepository.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((Product)null);
        _mockMongoRepository.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((Product)null);

        await _productService.AddProductAsync(_testProduct);

        _mockSqlRepository.Verify(x => x.AddProductAsync(It.Is<Product>(p => p.ProductName == _testProduct.ProductName)), Times.Once);
        _mockMongoRepository.Verify(x => x.AddProductAsync(It.Is<Product>(p => p.ProductName == _testProduct.ProductName)), Times.Once);
    }

    /// <summary>
    /// Test for the GetProductByNameAsync method. It retrieves a product by its name.
    /// </summary>
    [Fact]
    public async Task GetProductByNameAsync_ReturnsCorrectProduct()
    {
        _mockSqlRepository.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(_testProduct);

        var result = await _productService.GetProductByNameAsync(_testProduct.ProductName);

        Assert.Equal(_testProduct.ProductName, result.ProductName);
        _mockSqlRepository.Verify(x => x.GetProductByNameAsync(It.Is<string>(n => n == _testProduct.ProductName)), Times.Once);
    }

    /// <summary>
    /// Test for the UpdateProductAsync method. It updates a product in repositories.
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_CallsUpdateProductAsyncOnRepositories()
    {
        _mockSqlRepository.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(_testProduct);

        await _productService.UpdateProductAsync(_testProduct);

        _mockMongoRepository.Verify(x => x.UpdateProductAsync(It.Is<Product>(p => p.ProductName == _testProduct.ProductName)), Times.Once);
        _mockSqlRepository.Verify(x => x.UpdateProductAsync(It.Is<Product>(p => p.ProductName == _testProduct.ProductName)), Times.Once);
    }
}
