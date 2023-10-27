using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Services;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Tests.ServiceTests;
public class GameServiceTests
{
    private readonly DbContextOptions<GamestoreContext> _options;

    public GameServiceTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: "Gamestore")
            .Options;
        _context = new GamestoreContext(options);
        _gameService = new GameService(_context);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldReturnTrue_WhenGameIsCreated()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var gameService = new GameService(context);
        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A fun game",
            Genre = null,
            Platforms = null,
        };

        // Act
        var (isSuccess, errorMessage) = await gameService.CreateGameAsync(game);

        // Assert
        Assert.True(isSuccess);
        Assert.Null(errorMessage);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldReturnFalse_WhenGameAliasIsNotUnique()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var gameService = new GameService(context);
        var game1 = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A fun game",
            Genre = null,
            Platforms = null,
        };
        var game2 = new Game
        {
            GameAlias = "game1",
            Name = "Game 2",
            Description = "Another fun game",
            Genre = null,
            Platforms = null,
        };
        await gameService.CreateGameAsync(game1);

        // Act
        var (isSuccess, errorMessage) = await gameService.CreateGameAsync(game2);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("Game alias must be unique", errorMessage);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldReturnFalse_WhenDatabaseErrorOccurs()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        context.Database.EnsureCreated(); // create the database if it does not exist
        var gameService = new GameService(context);
        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A fun game",
            Genre = null,
            Platforms = null,
        };

        // Act
        var (isSuccess, errorMessage) = await gameService.CreateGameAsync(game);

        // Assert
        Assert.False(isSuccess);
        Assert.StartsWith("Database error:", errorMessage);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldReturnFalse_WhenUnknownErrorOccurs()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var gameService = new GameService(context);
        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A fun game",
            Genre = null,
            Platforms = null,
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();
        context.Entry(game).State = EntityState.Detached; // simulate an unknown error

        // Act
        var (isSuccess, errorMessage) = await gameService.CreateGameAsync(game);

        // Assert
        Assert.False(isSuccess);
        Assert.StartsWith("An error occurred:", errorMessage);
    }
}