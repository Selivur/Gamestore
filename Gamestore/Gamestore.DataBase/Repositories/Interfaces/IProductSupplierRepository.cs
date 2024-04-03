using Gamestore.Database.Entities.MongoDB;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Defines the contract for a product supplier repository.
/// </summary>
public interface IProductSupplierRepository
{
    /// <summary>
    /// Gets all product suppliers.
    /// </summary>
    /// <returns>A list of all product suppliers.</returns>
    Task<IEnumerable<ProductSupplier>> GetAllProductSuppliersAsync();

    /// <summary>
    /// Gets a product supplier by its Company Name.
    /// </summary>
    /// <param name="companyName">The ID of the product supplier.</param>
    /// <returns>The product supplier with the given ID, or null if no such product supplier exists.</returns>
    Task<ProductSupplier> GetProductSupplierByCompanyNameAsync(string companyName);

    /// <summary>
    /// Adds a new product supplier.
    /// </summary>
    /// <param name="productSupplier">The product supplier to add.</param>
    Task AddProductSupplierAsync(ProductSupplier productSupplier);

    /// <summary>
    /// Updates an existing product supplier.
    /// </summary>
    /// <param name="productSupplier">The product supplier to update.</param>
    Task UpdateProductSupplierAsync(ProductSupplier productSupplier);

    /// <summary>
    /// Deletes a product supplier.
    /// </summary>
    /// <param name="id">The ID of the product supplier to delete.</param>
    Task DeleteProductSupplierAsync(int id);
}
