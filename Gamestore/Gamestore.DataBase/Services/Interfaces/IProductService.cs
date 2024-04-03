using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Services.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Adds a new product to the databases.
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <returns>The product that was added.</returns>
    Task<Product> AddProductAsync(Product product);

    /// <summary>
    /// Gets a product by its name.
    /// </summary>
    /// <param name="name">The name of the product.</param>
    /// <returns>The product with the given name, or null if no such product exists.</returns>
    Task<Product> GetProductByNameAsync(string name);

    /// <summary>
    /// Updates an existing product in the databases.
    /// </summary>
    /// <param name="product">The product to update.</param>
    Task UpdateProductAsync(Product product);
}