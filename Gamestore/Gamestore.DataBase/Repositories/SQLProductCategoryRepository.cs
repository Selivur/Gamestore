using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;
public class SQLProductCategoryRepository : IProductCategoryRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLProductCategoryRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SQLProductCategoryRepository(GamestoreContext context)
    {
        _context = context;
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
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateProductCategoryAsync(ProductCategory productCategory)
    {
        _context.Entry(productCategory).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteProductCategoryAsync(int id)
    {
        var productCategory = await _context.ProductCategories.FindAsync(id);
        if (productCategory != null)
        {
            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
        }
    }
}