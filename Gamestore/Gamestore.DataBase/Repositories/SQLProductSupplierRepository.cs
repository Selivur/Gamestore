using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Represents a repository for managing product suppliers in a SQL database.
/// </summary>
public class SQLProductSupplierRepository : IProductSupplierRepository
{
    private readonly GamestoreContext _context;
    private readonly IDataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductSupplierRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SQLProductSupplierRepository(GamestoreContext context, IDataBaseLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductSupplier>> GetAllProductSuppliersAsync()
    {
        return await _context.ProductSuppliers.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProductSupplier> GetProductSupplierByCompanyNameAsync(string companyName)
    {
        return await _context.ProductSuppliers.SingleAsync(ps => ps.CompanyName == companyName);
    }

    /// <inheritdoc />
    public async Task AddProductSupplierAsync(ProductSupplier productSupplier)
    {
        _context.ProductSuppliers.Add(productSupplier);
        await SaveChangesAsync("Error when adding the  product supplier in the database.", CrudOperation.Add, null, productSupplier.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateProductSupplierAsync(ProductSupplier productSupplier)
    {
        var oldObject = await _context.ProductSuppliers.AsNoTracking().FirstAsync(o => o.SupplierId == productSupplier.SupplierId);
        _context.Entry(productSupplier).State = EntityState.Modified;
        await SaveChangesAsync("Error when updating the  product supplier in the database.", CrudOperation.Update, oldObject.ToBsonDocument(), productSupplier.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task DeleteProductSupplierAsync(int id)
    {
        var productSupplier = await _context.ProductSuppliers.FindAsync(id);
        if (productSupplier != null)
        {
            _context.ProductSuppliers.Remove(productSupplier);
            await SaveChangesAsync("Error when deleting the  product supplier in the database.", CrudOperation.Delete, productSupplier.ToBsonDocument(), null);
        }
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage, CrudOperation operation, BsonDocument oldObject, BsonDocument newObject)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }

        _logger.LogChange(
            action: operation,
            entityType: typeof(ProductSupplier).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}