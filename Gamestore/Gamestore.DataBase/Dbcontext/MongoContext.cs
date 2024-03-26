﻿using Gamestore.Database.Entities.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Gamestore.Database.Dbcontext;

/// <summary>
/// Represents the MongoDB context.
/// </summary>
public class MongoContext
{
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoContext"/> class.
    /// </summary>
    /// <param name="settings">The MongoDB settings.</param>
    public MongoContext(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        if (client != null)
        {
            _database = client.GetDatabase(settings.Value.Database);
        }
    }

    /// <summary>
    /// Gets the collection of shippers.
    /// </summary>
    public IMongoCollection<ProductShipper> ProductShippers
    {
        get { return _database.GetCollection<ProductShipper>("shippers"); }
    }

    /// <summary>
    /// Gets the collection of orders.
    /// </summary>
    public IMongoCollection<ProductOrder> ProductOrders
    {
        get { return _database.GetCollection<ProductOrder>("orders"); }
    }

    /// <summary>
    /// Gets the collection of products.
    /// </summary>
    public IMongoCollection<Product> Products
    {
        get { return _database.GetCollection<Product>("products"); }
    }

    /// <summary>
    /// Gets the collection of categories.
    /// </summary>
    public IMongoCollection<ProductCategory> ProductCategories
    {
        get { return _database.GetCollection<ProductCategory>("categories"); }
    }

    /// <summary>
    /// Gets the collection of order details.
    /// </summary>
    public IMongoCollection<ProductOrderDetails> ProductOrderDetails
    {
        get { return _database.GetCollection<ProductOrderDetails>("orderDetails"); }
    }

    /// <summary>
    /// Gets the collection of suppliers.
    /// </summary>
    public IMongoCollection<ProductSupplier> ProductSuppliers
    {
        get { return _database.GetCollection<ProductSupplier>("suppliers"); }
    }

    /// <summary>
    /// Gets the collection of logs.
    /// </summary>
    public IMongoCollection<Log> Logs
    {
        get { return _database.GetCollection<Log>("logs"); }
    }
}