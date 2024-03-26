using Gamestore.Database.Entities;
using Gamestore.Database.Entities.MongoDB;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Dbcontext;

/// <summary>
/// Represents the database context for the Gamestore application.
/// </summary>
public class GamestoreContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GamestoreContext"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the context.</param>
    public GamestoreContext(DbContextOptions<GamestoreContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for games in the database.
    /// </summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for genres in the database.
    /// </summary>
    public DbSet<Genre> Genres { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for platforms in the database.
    /// </summary>
    public DbSet<Platform> Platforms { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for publishers in the database.
    /// </summary>
    public DbSet<Publisher> Publishers { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for orders in the database.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for order details in the database.
    /// </summary>
    public DbSet<OrderDetails> OrderDetails { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for customers in the database.
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for comments in the database.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// Gets or sets the collection of products.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Gets or sets the collection of product suppliers.
    /// </summary>
    public DbSet<ProductSupplier> ProductSuppliers { get; set; }

    /// <summary>
    /// Gets or sets the collection of product order details.
    /// </summary>
    public DbSet<ProductOrderDetails> ProductOrderDetails { get; set; }

    /// <summary>
    /// Gets or sets the collection of product categories.
    /// </summary>
    public DbSet<ProductCategory> ProductCategories { get; set; }

    /// <summary>
    /// Gets or sets the collection of product orders.
    /// </summary>
    public DbSet<ProductOrder> ProductOrders { get; set; }

    /// <summary>
    /// Gets or sets the collection of product shippers.
    /// </summary>
    public DbSet<ProductShipper> ProductShippers { get; set; }
}