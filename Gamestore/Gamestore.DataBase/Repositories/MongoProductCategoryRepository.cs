using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;
public class MongoProductCategoryRepository : IProductCategoryRepository
{
    private readonly MongoContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductCategoryRepository"/> class.
    /// </summary>
    /// <param name="context">The MongoDB context.</param>
    public MongoProductCategoryRepository(MongoContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductCategory>> GetAllProductCategoriesAsync()
    {
        return await _context.ProductCategories.Find(_ => true).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProductCategory> GetProductCategoryByIdAsync(int id)
    {
        return await _context.ProductCategories.Find(p => p.CategoryID == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task AddProductCategoryAsync(ProductCategory productCategory)
    {
        await _context.ProductCategories.InsertOneAsync(productCategory);
    }

    /// <inheritdoc />
    public async Task UpdateProductCategoryAsync(ProductCategory productCategory)
    {
        await _context.ProductCategories.ReplaceOneAsync(p => p.CategoryID == productCategory.CategoryID, productCategory);
    }

    /// <inheritdoc />
    public async Task DeleteProductCategoryAsync(int id)
    {
        await _context.ProductCategories.DeleteOneAsync(p => p.CategoryID == id);
    }
}