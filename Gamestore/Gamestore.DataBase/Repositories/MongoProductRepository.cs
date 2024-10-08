﻿using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Represents a repository for managing products in a MongoDB database.
/// </summary>
public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoProductRepository"/> class.
    /// </summary>
    /// <param name="context">The MongoDB context.</param>
    public MongoProductRepository(IMongoCollection<Product> context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Find(_ => true).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Product?> GetProductByNameAsync(string name)
    {
        return await _context.Find(p => p.ProductName == name).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task AddProductAsync(Product product)
    {
        await _context.InsertOneAsync(product);
    }

    /// <inheritdoc />
    public async Task UpdateProductAsync(Product product)
    {
        await _context.ReplaceOneAsync(p => p.ProductID == product.ProductID, product);
    }

    /// <inheritdoc />
    public async Task DeleteProductAsync(int id)
    {
        await _context.DeleteOneAsync(p => p.ProductID == id);
    }
}
