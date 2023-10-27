using Gamestore.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Dbcontext;

public class GamestoreContext : DbContext
{
    public GamestoreContext(DbContextOptions<GamestoreContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Platform> Platforms { get; set; }
}
