namespace Gamestore.Tests.Database.RepositoriesTests;
public class GameRepositoryTests
{
    private readonly DbContextOptions<GamestoreContext> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepositoryTests"/> class.
    /// </summary>
    public GameRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GameRepository
    /// returns a game when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnGame()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A test game",
        };
        context.Games.Add(game);
        context.SaveChanges();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game.Id, result.Id);
        Assert.Equal(game.Name, result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GameRepository
    /// returns null when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByAliasAsync method of the GameRepository returns a game when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByAliasAsync_ShouldReturnGame()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var gameAlias = "test-game";
        var game = new Game { GameAlias = gameAlias, Name = "Test Game" };
        context.Games.Add(game);
        context.SaveChanges();

        // Act
        var result = await repository.GetByAliasAsync(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameAlias, result.GameAlias);
        Assert.Equal(game.Name, result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByAliasAsync method of the GameRepository returns null when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByAliasAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var gameAlias = "test-game";

        // Act
        var result = await repository.GetByAliasAsync(gameAlias);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByAliasAsync method of the GameRepository returns null when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByAliasAsync_ShouldReturnNullIfGameNotFound()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var gameAlias = "test-game";

        // Act
        var result = await repository.GetByAliasAsync(gameAlias);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetAllAsync method of the GameRepository returns all games from the database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var games = new List<Game>
        {
            new() { Id = 1, GameAlias = "game-1", Name = "Game 1", Description = "Description 1" },
            new() { Id = 2, GameAlias = "game-2", Name = "Game 2", Description = "Description 2" },
            new() { Id = 3, GameAlias = "game-3", Name = "Game 3", Description = "Description 3" },
        };

        context.Games.AddRange(games);
        context.SaveChanges();

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(games.Count, result.Count);

        foreach (var game in games)
        {
            Assert.Contains(result, g => g.GameAlias == game.GameAlias && g.Name == game.Name);
        }
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GameRepository adds a game to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddGameToDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var game = new Game
        {
            GameAlias = "new-game",
            Name = "New Game",
            Description = "A new game",
        };

        // Act
        await repository.AddAsync(game);

        // Assert
        var savedGame = await context.Games.FirstOrDefaultAsync(g => g.GameAlias == "new-game");
        Assert.NotNull(savedGame);
        Assert.Equal(game.Name, savedGame.Name);
    }

    /// <summary>
    /// Test to verify that the UpdateAsync method of the GameRepository updates a game in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateGameInDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A test game",
        };

        context.Games.Add(game);
        context.SaveChanges();

        // Modify the game
        game.Name = "Updated Game Name";
        game.Description = "Updated game description";

        // Act
        await repository.UpdateAsync(game);

        // Assert
        var updatedGame = await context.Games.FirstOrDefaultAsync(g => g.GameAlias == "game1");
        Assert.NotNull(updatedGame);
        Assert.Equal("Updated Game Name", updatedGame.Name);
        Assert.Equal("Updated game description", updatedGame.Description);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GameRepository removes a game from the database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemoveGameFromDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GameRepository(context);

        var game = new Game
        {
            GameAlias = "game1",
            Name = "Game 1",
            Description = "A test game",
        };

        context.Games.Add(game);
        context.SaveChanges();

        // Act
        await repository.RemoveAsync("game1");

        // Assert
        var removedGame = await context.Games.FirstOrDefaultAsync(g => g.GameAlias == "game1");
        Assert.Null(removedGame);
    }
}
