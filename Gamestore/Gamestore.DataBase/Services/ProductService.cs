using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services.Interfaces;

namespace Gamestore.Database.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository _sqlProductRepository;
    private readonly IProductRepository _mongoProductRepository;

    public ProductService(IProductRepository sqlProductRepository, IProductRepository mongoProductRepository)
    {
        _sqlProductRepository = sqlProductRepository;
        _mongoProductRepository = mongoProductRepository;
    }

    /// <inheritdoc />
    public async Task<Product> AddProductAsync(Product product)
    {
        var existingProductSql = await _sqlProductRepository.GetProductByNameAsync(product.ProductName);
        var existingProductMongo = await _mongoProductRepository.GetProductByNameAsync(product.ProductName);

        if (existingProductSql != null)
        {
            throw new InvalidOperationException($"Product with name {product.ProductName} already exists.");
        }

        await _sqlProductRepository.AddProductAsync(product);

        if (existingProductMongo == null)
        {
            await _mongoProductRepository.AddProductAsync(product);
        }
        else
        {
            existingProductMongo.UnitsInStock = product.UnitsInStock;
            await _mongoProductRepository.UpdateProductAsync(existingProductMongo);
        }

        return product;
    }

    /// <inheritdoc />
    public async Task<Product> GetProductByNameAsync(string name)
    {
        var product = await _sqlProductRepository.GetProductByNameAsync(name) ?? await _mongoProductRepository.GetProductByNameAsync(name);

        return product;
    }

    /// <inheritdoc />
    public async Task UpdateProductAsync(Product product)
    {
        await _mongoProductRepository.UpdateProductAsync(product);

        var existingProduct = await _sqlProductRepository.GetProductByNameAsync(product.ProductName);

        if (existingProduct != null)
        {
            await _sqlProductRepository.UpdateProductAsync(product);
        }
        else
        {
            await _sqlProductRepository.AddProductAsync(product);
        }
    }
}