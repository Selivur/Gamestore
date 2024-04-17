using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Represents a repository for managing products in a SQL database.
/// </summary>
public class SQLProductRepository : IProductRepository
{
    private readonly GamestoreContext _context;
    private readonly IDataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SQLProductRepository(GamestoreContext context, IDataBaseLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Product?> GetProductByNameAsync(string name)
    {
        return await _context.Products.SingleOrDefaultAsync(db => db.ProductName == name);
    }

    /// <inheritdoc />
    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await SaveChangesAsync("Error when adding the product in the database.", CrudOperation.Add, null, product.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateProductAsync(Product product)
    {
        var oldObject = await _context.Products.AsNoTracking().FirstAsync(o => o.ProductID == product.ProductID);
        _context.Entry(product).State = EntityState.Modified;
        await SaveChangesAsync("Error when updating the product in the database.", CrudOperation.Update, oldObject.ToBsonDocument(), product.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await SaveChangesAsync("Error when deleting the product in the database.", CrudOperation.Delete, product.ToBsonDocument(), null);
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
            entityType: typeof(Product).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}