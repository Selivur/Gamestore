using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Defines the contract for a product category repository.
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// Gets all product categories.
    /// </summary>
    /// <returns>A list of all product categories.</returns>
    Task<IEnumerable<ProductCategory>> GetAllProductCategoriesAsync();

    /// <summary>
    /// Gets a product category by its ID.
    /// </summary>
    /// <param name="id">The ID of the product category.</param>
    /// <returns>The product category with the given ID, or null if no such product category exists.</returns>
    Task<ProductCategory> GetProductCategoryByIdAsync(int id);

    /// <summary>
    /// Adds a new product category.
    /// </summary>
    /// <param name="productCategory">The product category to add.</param>
    Task AddProductCategoryAsync(ProductCategory productCategory);

    /// <summary>
    /// Updates an existing product category.
    /// </summary>
    /// <param name="productCategory">The product category to update.</param>
    Task UpdateProductCategoryAsync(ProductCategory productCategory);

    /// <summary>
    /// Deletes a product category.
    /// </summary>
    /// <param name="id">The ID of the product category to delete.</param>
    Task DeleteProductCategoryAsync(int id);
}
