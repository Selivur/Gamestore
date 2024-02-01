namespace Gamestore.Tests.Database.RepositoriesTests;
public class GameRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly GameRepository _gameRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepositoryTests"/> class.
    /// </summary>
    public GameRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GamestoreContext(_options);
        _gameRepository = new GameRepository(_context);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GameRepository
    /// returns a game when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnGame_WhenGameExists()
    {
        // Act
        var result = await _gameRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GameRepository
    /// returns null when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Act
        var result = await _gameRepository.GetByIdAsync(4);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByAliasAsync method of the GameRepository returns a game when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByAliasAsync_ShouldReturnGame_WhenGameExists()
    {
        // Act
        var result = await _gameRepository.GetByAliasAsync("game1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("game1", result.GameAlias);
    }

    /// <summary>
    /// Test to verify that the GetByAliasAsync method of the GameRepository returns null when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByAliasAsync_ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Act
        var result = await _gameRepository.GetByAliasAsync("game4");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetAllAsync method of the GameRepository returns all games from the database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        // Act
        var result = await _gameRepository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GameRepository adds a game to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddGameToDatabase()
    {
        // Arrange
        var newGame = new Game
        {
            GameAlias = "new-game",
            Name = "New Game",
            Description = "A new game",
        };

        // Act
        await _gameRepository.AddAsync(newGame);

        var savedGame = await _context.Games.FirstOrDefaultAsync(g => g.GameAlias == "new-game");

        // Assert
        Assert.NotNull(savedGame);
        Assert.Equal(newGame.Name, savedGame.Name);
    }

    /// <summary>
    /// Test to verify that the UpdateAsync method of the GameRepository updates a game in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateGameInDatabase()
    {
        // Arrange
        var gameToUpdate = await _context.Games.FirstOrDefaultAsync(g => g.Id == 1);
        gameToUpdate.Name = "Updated Game";
        gameToUpdate.Description = "Updated Description";

        // Act
        await _gameRepository.UpdateAsync(gameToUpdate);

        var updatedGame = await _context.Games.FirstOrDefaultAsync(g => g.Id == 1);

        // Assert
        Assert.NotNull(updatedGame);
        Assert.Equal("Updated Game", updatedGame.Name);
        Assert.Equal("Updated Description", updatedGame.Description);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GameRepository removes a game from the database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemoveGameFromDatabase()
    {
        // Act
        await _gameRepository.RemoveAsync("game1");

        var removedGame = await _context.Games.FirstOrDefaultAsync(g => g.GameAlias == "game1");

        // Assert
        Assert.Null(removedGame);
    }

    /// <summary>
    /// Checks that the AddGameWithDependencies method correctly adds games with dependencies to the database.
    /// </summary>
    [Fact]
    public async Task UpdateGameWithDependencies_AddsGameToDbWithCorrectDependencies()
    {
        // Arrange
        var game = new Game
        {
            GameAlias = "game-with-dependencies",
            Name = "Game with dependencies",
            Description = "A game with dependencies",
        };
        await _gameRepository.AddAsync(game);
        var addedGame = await _context.Games.AsNoTracking().FirstAsync(g => g.GameAlias == "game-with-dependencies");

        var genresId = new[] { 1, 2 };
        var platformsId = new[] { 1, 2 };
        var publisherId = 3;

        // Act
        await _gameRepository.UpdateGameWithDependencies(addedGame, genresId, platformsId, publisherId);

        // Assert
        var updatedGame = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.GameAlias == "game-with-dependencies");

        Assert.NotNull(updatedGame);
        Assert.Equal(genresId.Length, updatedGame.Genres.Count);
        Assert.Equal(platformsId.Length, updatedGame.Platforms.Count);
        Assert.Equal(publisherId, updatedGame.Publisher.Id);
    }

    /// <summary>
    /// Test for adding a game with its associated dependencies.
    /// This method is expected to add the game with the provided genres, platforms and publisher to the database.
    /// </summary>
    [Fact]
    public async Task AddGameWithDependencies_AddsGameToDbWithCorrectDependencies()
    {
        // Arrange
        var newGame = new Game
        {
            GameAlias = "new-game-with-dependencies",
            Name = "New Game with dependencies",
            Description = "A new game with dependencies",
        };

        var genresId = new[] { 1, 2 };
        var platformsId = new[] { 1, 2 };
        var publisherId = 3;

        // Act
        await _gameRepository.AddGameWithDependencies(newGame, genresId, platformsId, publisherId);

        // Assert
        var addedGame = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.GameAlias == "new-game-with-dependencies");

        // Assert
        Assert.NotNull(addedGame);
        Assert.Equal(genresId.Length, addedGame.Genres.Count);
        Assert.Equal(platformsId.Length, addedGame.Platforms.Count);
        Assert.Equal(publisherId, addedGame.Publisher.Id);
    }

    /// <summary>
    /// Test for updating game with dependencies.
    /// It should add game with provided genres, platforms and publisher to the database.
    /// </summary>
    [Fact]
    public async Task UpdateGameWithDependencies_AddsGameWithDependenciesToDatabase()
    {
        // Arrange
        var newGame = new Game
        {
            GameAlias = "updated-game",
            Name = "Updated Game",
            Description = "An updated game with dependencies",
        };
        await _gameRepository.AddAsync(newGame);

        var addedGame = await _context.Games.SingleAsync(g => g.GameAlias == "updated-game");

        var testGenres = new List<int> { 1, 2 };
        var testPlatforms = new List<int> { 1, 2 };
        var testPublisher = 3;

        // Act
        await _gameRepository.UpdateGameWithDependencies(addedGame, testGenres.ToArray(), testPlatforms.ToArray(), testPublisher);

        var updatedGame = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .SingleAsync(g => g.GameAlias == "updated-game");

        // Assert
        Assert.Equal(testGenres.Count, updatedGame.Genres.Count);
        Assert.Equal(testPlatforms.Count, updatedGame.Platforms.Count);
        Assert.Equal(testPublisher, updatedGame.Publisher.Id);
    }

    /// <summary>
    /// Test for getting games by publisher name.
    /// This method is expected to return a list of games published by the given publisher.
    /// </summary>
    [Fact]
    public async Task GetByPublisherNameAsync_ReturnsGamesByPublisher()
    {
        // Arrange
        var publisherName = "TestCompanyName";

        // Act
        var games = await _gameRepository.GetByPublisherNameAsync(publisherName);

        // Assert
        Assert.All(games, g => Assert.Equal(publisherName, g.Publisher.CompanyName));
    }

    /// <summary>
    /// Test for getting games by genre ID.
    /// This method is expected to return a list of games of the given genre.
    /// </summary>
    [Fact]
    public async Task GetByGenreIdAsync_ReturnsGamesByGenre()
    {
        // Arrange
        var genreId = 1;

        // Arrange
        var games = await _gameRepository.GetByGenreIdAsync(genreId);

        // Assert
        Assert.All(games, g => Assert.Contains(g.Genres, genre => genre.Id == genreId));
    }

    /// <summary>
    /// Test for getting games by platform type.
    /// This method is expected to return a list of games that are available on the given platform type.
    /// </summary>
    [Fact]
    public async Task GetByPlatformTypeAsync_ReturnsGamesByPlatformType()
    {
        // Arrange
        var platformType = "PlatformType1"; // replace with an actual platform type from your test data.

        // Act
        var games = await _gameRepository.GetByPlatformTypeAsync(platformType);

        // Assert
        Assert.All(games, g => Assert.Contains(g.Platforms, platform => platform.Type == platformType));
    }

    /// <summary>
    /// Test for getting games by non-existing publisher name.
    /// This method is expected to throw an ArgumentException.
    /// </summary>
    [Fact]
    public async Task GetByPublisherNameAsync_ThrowsArgumentExceptionForNonExistingPublisher()
    {
        // Arrange
        var nonExistingPublisherName = "NonExistingPublisher";

        // Act
        var nonExistingPublisher = _gameRepository.GetByPublisherNameAsync(nonExistingPublisherName);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => nonExistingPublisher);
    }

    /// <summary>
    /// Test for getting games by non-existing genre ID.
    /// This method is expected to throw an ArgumentException.
    /// </summary>
    [Fact]
    public async Task GetByGenreIdAsync_ThrowsArgumentExceptionForNonExistingGenre()
    {
        // Arrange
        var nonExistingGenreId = -1;

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _gameRepository.GetByGenreIdAsync(nonExistingGenreId));
    }

    /// <summary>
    /// Test for getting games by non-existing platform type.
    /// This method is expected to throw an ArgumentException.
    /// </summary>
    [Fact]
    public async Task GetByPlatformTypeAsync_ThrowsArgumentExceptionForNonExistingPlatformType()
    {
        // Arrange
        var nonExistingPlatformType = "NonExistingPlatformType";

        // Act
        var nonExistingPlatform = _gameRepository.GetByPlatformTypeAsync(nonExistingPlatformType);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => nonExistingPlatform);
    }

    /// <summary>
    /// Clean up the resources used by the class.
    /// </summary>
    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }

    private void Cleanup()
    {
        foreach (var entity in _context.Games)
        {
            _context.Remove(entity);
        }

        _context.SaveChanges();
    }

    private void InitializeTestData()
    {
        var testGenres = new List<Genre>
        {
            new() { Id = 1, Name = "Genre1" },
            new() { Id = 2, Name = "Genre2" },
        };

        var testPlatforms = new List<Platform>
        {
            new() { Id = 1, Type = "PlatformType1" },
            new() { Id = 2, Type = "PlatformType2" },
        };

        var testPublisher = new Publisher { Id = 3, CompanyName = "TestCompanyName", Description = "TestDescription", HomePage = "TestUrl" };

        var testGames = new List<Game>
        {
            new() { Id = 1, GameAlias = "game1", Name = "Game 1", Description = "A test game" },
            new() { Id = 2, GameAlias = "game2", Name = "Game 2", Description = "A test game" },
            new() { Id = 3, GameAlias = "game3", Name = "Game 3", Description = "A test game" },
        };

        _context.AddRange(testGenres);
        _context.AddRange(testPlatforms);
        _context.Add(testPublisher);
        _context.AddRange(testGames);

        _context.SaveChanges();
    }
}
