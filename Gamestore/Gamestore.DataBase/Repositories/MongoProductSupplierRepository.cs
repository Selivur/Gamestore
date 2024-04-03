using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Represents a repository for managing product suppliers in a MongoDB database.
/// </summary>
public class MongoProductSupplierRepository : IProductSupplierRepository
{
    private readonly MongoContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductSupplierRepository"/> class.
    /// </summary>
    /// <param name="context">The MongoDB context.</param>
    public MongoProductSupplierRepository(MongoContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductSupplier>> GetAllProductSuppliersAsync()
    {
        return await _context.ProductSuppliers.Find(_ => true).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProductSupplier> GetProductSupplierByCompanyNameAsync(string companyName)
    {
        return await _context.ProductSuppliers.Find(p => p.CompanyName == companyName).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task AddProductSupplierAsync(ProductSupplier productSupplier)
    {
        await _context.ProductSuppliers.InsertOneAsync(productSupplier);
    }

    /// <inheritdoc />
    public async Task UpdateProductSupplierAsync(ProductSupplier productSupplier)
    {
        await _context.ProductSuppliers.ReplaceOneAsync(p => p.SupplierId == productSupplier.SupplierId, productSupplier);
    }

    /// <inheritdoc />
    public async Task DeleteProductSupplierAsync(int id)
    {
        await _context.ProductSuppliers.DeleteOneAsync(p => p.SupplierId == id);
    }
}