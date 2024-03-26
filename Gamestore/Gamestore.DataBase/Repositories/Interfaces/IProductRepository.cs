using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Defines the contract for a product repository.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <returns>The product with the given ID, or null if no such product exists.</returns>
    Task<Product> GetProductByIdAsync(int id);

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    Task AddProductAsync(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    Task UpdateProductAsync(Product product);

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    Task DeleteProductAsync(int id);

    // Add similar methods for suppliers and categories...
}
