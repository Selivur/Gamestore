using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories;
using Gamestore.Database.Services.Interfaces;

namespace Gamestore.Database.Services;
public class ProductService : IProductService
{
    private readonly SQLProductRepository _sqlProductRepository;
    private readonly MongoProductRepository _mongoProductRepository;

    public ProductService(SQLProductRepository sqlProductRepository, MongoProductRepository mongoProductRepository)
    {
        _sqlProductRepository = sqlProductRepository;
        _mongoProductRepository = mongoProductRepository;
    }

    /// <inheritdoc />
    public async Task<Product> AddProductAsync(Product product)
    {
        var existingProductSql = await _sqlProductRepository.GetProductByNameAsync(product.ProductName);
        var existingProductMongo = await _mongoProductRepository.GetProductByNameAsync(product.ProductName);

        if (existingProductSql == null && existingProductMongo == null)
        {
            await _sqlProductRepository.AddProductAsync(product);

            // await _mongoProductRepository.AddProductAsync(product);
        }
        else
        {
            throw new InvalidOperationException($"Product with name {product.ProductName} already exists.");
        }

        return product;
    }

    /// <inheritdoc />
    public async Task<Product> GetProductByNameAsync(string name)
    {
        return await _sqlProductRepository.GetProductByNameAsync(name);
    }

    /// <inheritdoc />
    public async Task UpdateProductAsync(Product product)
    {
        await _sqlProductRepository.UpdateProductAsync(product);
        await _mongoProductRepository.UpdateProductAsync(product);
    }
}