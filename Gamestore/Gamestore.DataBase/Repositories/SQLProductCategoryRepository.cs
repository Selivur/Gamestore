using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;
public class SQLProductCategoryRepository : IProductCategoryRepository
{
    private readonly GamestoreContext _context;
    private readonly DataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductCategoryRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SQLProductCategoryRepository(GamestoreContext context, MongoContext mongoContext)
    {
        _context = context;
        _logger = new DataBaseLogger(mongoContext);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductCategory>> GetAllProductCategoriesAsync()
    {
        return await _context.ProductCategories.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProductCategory> GetProductCategoryByIdAsync(int id)
    {
        return await _context.ProductCategories.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task AddProductCategoryAsync(ProductCategory productCategory)
    {
        _context.ProductCategories.Add(productCategory);
        await SaveChangesAsync("Error when adding the Product Category in the database.", CrudOperation.Add, null, productCategory.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task UpdateProductCategoryAsync(ProductCategory productCategory)
    {
        var oldObject = await _context.ProductCategories.AsNoTracking().FirstAsync(o => o.Id == productCategory.Id);
        _context.Entry(productCategory).State = EntityState.Modified;
        await SaveChangesAsync("Error when updating the Product Category in the database.", CrudOperation.Update, oldObject.ToBsonDocument(), productCategory.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task DeleteProductCategoryAsync(int id)
    {
        var productCategory = await _context.ProductCategories.FindAsync(id);
        if (productCategory != null)
        {
            _context.ProductCategories.Remove(productCategory);
            await SaveChangesAsync("Error when deleting the Product Category in the database.", CrudOperation.Delete, productCategory.ToBsonDocument(), null);
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
            entityType: typeof(ProductCategory).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}