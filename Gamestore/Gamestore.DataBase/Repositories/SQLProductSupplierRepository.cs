using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Represents a repository for managing product suppliers in a SQL database.
/// </summary>
public class SQLProductSupplierRepository : IProductSupplierRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductSupplierRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SQLProductSupplierRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductSupplier>> GetAllProductSuppliersAsync()
    {
        return await _context.ProductSuppliers.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProductSupplier> GetProductSupplierByIdAsync(int id)
    {
        return await _context.ProductSuppliers.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task AddProductSupplierAsync(ProductSupplier productSupplier)
    {
        _context.ProductSuppliers.Add(productSupplier);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateProductSupplierAsync(ProductSupplier productSupplier)
    {
        _context.Entry(productSupplier).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteProductSupplierAsync(int id)
    {
        var productSupplier = await _context.ProductSuppliers.FindAsync(id);
        if (productSupplier != null)
        {
            _context.ProductSuppliers.Remove(productSupplier);
            await _context.SaveChangesAsync();
        }
    }
}