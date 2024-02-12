using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Game;
using Gamestore.Api.Services;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="GameService"/> class.
/// </summary>
public class GameServiceTests
{
    private readonly Mock<IGameRepository> _mockRepository;
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly GameService _gameService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServiceTests"/> class.
    /// </summary>
    public GameServiceTests()
    {
        _mockRepository = new Mock<IGameRepository>();
        _mockCommentService = new Mock<ICommentService>();
        _gameService = new GameService(_mockRepository.Object, _mockCommentService.Object);
    }

    /// <summary>
    /// Positive test that verifies whether the CreateGameAsync method of the GameService correctly
    /// creates a new game when the provided game alias is unique.
    /// </summary>
    [Fact]
    public async Task CreateGameAsync_ShouldAddNewGame()
    {
        // Arrange
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Name = "Valid Name", Description = "Desc", GameAlias = "valid_name" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };

        _mockRepository.Setup(repo => repo.GetByAliasAsync(validGameRequest.GameRequest.GameAlias!))
            .ReturnsAsync((Game)null);

        bool addAsyncCalled = false;

        _mockRepository.Setup(repo => repo.AddGameWithDependencies(It.IsAny<Game>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int>()))
            .Callback((Game game, int[] genresId, int[] platformsId, int publisherId) =>
            {
                addAsyncCalled = true;
            });

        // Act
        await _gameService.CreateGameAsync(validGameRequest);

        // Assert
        _mockRepository.Verify(repo => repo.GetByAliasAsync(validGameRequest.GameRequest.GameAlias!), Times.Once);
        Assert.True(addAsyncCalled, "AddAsync should have been called within AddGameWithDependencies");
    }

    /// <summary>
    /// Positive test that verifies whether the CreateGameAsync method of the GameService
    /// correctly returns an error message when the provided game alias is not unique.
    /// </summary>
    [Fact]
    public async Task CreateGameAsync_ShouldThrowException_WhenGameAliasNotUnique()
    {
        // Arrange
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Name = "Valid Name", Description = "Desc", GameAlias = "valid_name" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };

        var existingGame = new Game
        {
            GameAlias = validGameRequest.GameRequest.GameAlias!,
            Name = "Test Game",
            Description = "Existing game description",
        };

        _mockRepository.Setup(repo => repo.GetByAliasAsync(validGameRequest.GameRequest.GameAlias!))
            .ReturnsAsync(existingGame);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.CreateGameAsync(validGameRequest));
    }

    /// <summary>
    /// Tests the <see cref="GameService.GetGameResponseByAliasAsync"/> method to ensure it returns the expected <see cref="GameResponse"/> when the game exists.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_ShouldReturnGameResponse_WhenGameExists()
    {
        // Arrange
        var gameAlias = "test-game";
        var existingGame = new Game
        {
            GameAlias = gameAlias,
            Name = "Test Game",
            Description = "A test game description",
        };

        _mockRepository.Setup(repo => repo.GetByAliasAsync(gameAlias))
            .ReturnsAsync(existingGame);

        // Act
        var result = await _gameService.GetGameResponseByAliasAsync(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingGame.GameAlias, result.Key);
        Assert.Equal(existingGame.Name, result.Name);
        Assert.Equal(existingGame.Description, result.Description);
    }

    /// <summary>
    /// Tests the <see cref="GameService.GetGameResponseByAliasAsync"/> method to ensure it throws a <see cref="KeyNotFoundException"/> when the game is not found.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_ShouldThrowException_WhenGameNotFound()
    {
        // Arrange
        var gameAlias = "non-existing-game";

        _mockRepository.Setup(repo => repo.GetByAliasAsync(gameAlias))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameResponseByAliasAsync(gameAlias));
    }

    /// <summary>
    /// Positive test that verifies whether the UpdateGameAsync method of the
    /// GameService correctly updates an existing game with valid input data.
    /// </summary>
    [Fact]
    public async Task UpdateGameAsync_ShouldUpdateExistingGame()
    {
        // Arrange
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Id = "1", Name = "new Name", Description = "Upd Desc", GameAlias = "valid_name" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };

        var existingGame = new Game
        {
            Id = 1,
            GameAlias = validGameRequest.GameRequest.GameAlias!,
            Name = "Test Game",
            Description = "A test game description",
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(existingGame);

        // Act
        await _gameService.UpdateGameAsync(validGameRequest);

        // Assert
        _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mockRepository.Verify(repo => repo.UpdateGameWithDependencies(It.IsAny<Game>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int>()), Times.Once);

        // Additional assertions
        Assert.Equal(validGameRequest.GameRequest.Name, existingGame.Name);
        Assert.Equal(validGameRequest.GameRequest.Description, existingGame.Description);
    }

    /// <summary>
    /// Positive test that verifies whether the UpdateGameAsync method of the
    /// GameService correctly returns an error message when attempting to update a game with a non-existent alias.
    /// </summary>
    [Fact]
    public async Task UpdateGameAsync_ShouldThrowException_WhenGameNotFound()
    {
        // Arrange
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Name = "Name", Description = "Desc", GameAlias = "non_ex_game" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };

        _mockRepository.Setup(repo => repo.GetByAliasAsync(validGameRequest.GameRequest.GameAlias!))
                      .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.UpdateGameAsync(validGameRequest));
    }

    /// <summary>
    /// Positive test that verifies whether the RemoveGameAsync method
    /// of the GameService correctly removes an existing game by its alias.
    /// </summary>
    [Fact]
    public async Task RemoveGameAsync_ShouldRemoveExistingGame()
    {
        // Arrange
        var gameAlias = "test-game";

        _mockRepository.Setup(repo => repo.RemoveAsync(gameAlias))
            .Returns(Task.CompletedTask);

        // Act
        await _gameService.RemoveGameAsync(gameAlias);

        // Assert
        _mockRepository.Verify(repo => repo.RemoveAsync(gameAlias), Times.Once);
    }

    /// <summary>
    /// Test that checks whether the RemoveGameAsync method of the GameService
    /// correctly handles an exception during execution and returns the appropriate error message.
    /// </summary>
    [Fact]
    public async Task RemoveGameAsync_ShouldThrowException_WhenGameNotFound()
    {
        // Arrange
        var gameAlias = "non-existing-game";

        _mockRepository.Setup(repo => repo.RemoveAsync(gameAlias))
            .ThrowsAsync(new InvalidOperationException("Game not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.RemoveGameAsync(gameAlias));
        Assert.Equal("Game not found", exception.Message);
    }

    /// <summary>
    /// Test to verify that the GetAllGamesAsync method of the GameService
    /// retrieves and correctly maps games from the repository to GameResponse objects.
    /// </summary>
    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnAllGames()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { GameAlias = "game1", Name = "Game 1", Description = "Description 1" },
            new() { GameAlias = "game2", Name = "Game 2", Description = "Description 2" },
            new() { GameAlias = "game3", Name = "Game 3", Description = "Description 3" },
        };

        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetAllGamesAsync();

        // Assert
        Assert.Equal(games.Count, result.Count());

        for (var i = 0; i < games.Count; i++)
        {
            Assert.Equal(games[i].GameAlias, result.ElementAt(i).Key);
            Assert.Equal(games[i].Name, result.ElementAt(i).Name);
            Assert.Equal(games[i].Description, result.ElementAt(i).Description);
        }
    }

    /// <summary>
    /// Tests the <see cref="GameService.GetAllGamesAsync"/> method to ensure it returns an empty list when no games are found.
    /// </summary>
    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnEmptyList_WhenNoGamesFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Game>());

        // Act
        var result = await _gameService.GetAllGamesAsync();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Test for GetGameByIdAsync method with valid game ID. Expects to return a GameResponse.
    /// </summary>
    [Fact]
    public async Task GetGameByIdAsync_WithValidId_ReturnsGameResponse()
    {
        // Arrange
        var testGameId = 1;
        var testGame = new Game { Id = testGameId };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(testGameId))
            .ReturnsAsync(testGame);

        // Act
        var gameResponse = await _gameService.GetGameByIdAsync(testGameId);

        // Assert
        Assert.NotNull(gameResponse);
        Assert.Equal(testGameId.ToString(), gameResponse.Id);
    }

    /// <summary>
    /// Test for GetGameByIdAsync method with game ID not in the repository. Expect KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task GetGameByIdAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        int nonExistentId = 99;

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByIdAsync(nonExistentId));
    }

    /// <summary>
    /// Test for GetGameByAliasAsync method with valid game alias. Expects to return a Game.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_WithValidAlias_ReturnsGame()
    {
        // Arrange
        var gameAlias = "testAlias";
        var testGame = new Game { GameAlias = gameAlias };

        _mockRepository
            .Setup(repository => repository.GetByAliasAsync(gameAlias))
            .ReturnsAsync(testGame);

        // Act
        var returnedGame = await _gameService.GetGameByAliasAsync(gameAlias);

        // Assert
        Assert.NotNull(returnedGame);
        Assert.Equal(gameAlias, returnedGame.GameAlias);
    }

    /// <summary>
    /// Test for GetGameByAliasAsync method with game alias not in the repository. Expect KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_WithNonExistentAlias_ThrowsKeyNotFoundException()
    {
        // Arrange
        string nonExistentAlias = "nonExistentAlias";

        _mockRepository
            .Setup(repository => repository.GetByAliasAsync(nonExistentAlias))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByAliasAsync(nonExistentAlias));
    }

    /// <summary>
    /// Test for UpdateGameWithoutDependenciesAsync with valid game id.
    /// Expects the updated game will be passed to the repository's UpdateAsync method.
    /// </summary>
    [Fact]
    public async Task UpdateGameWithoutDependenciesAsync_WithValidId_UpdatesGameInRepository()
    {
        // Arrange
        var existingGame = new Game { Id = 1, Name = "OldName" };
        var inputGame = new Game { Id = 1, Name = "NewName" };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(existingGame.Id))
            .ReturnsAsync(existingGame);
        _mockRepository
            .Setup(repository => repository.UpdateAsync(It.Is<Game>(g => g.Name == inputGame.Name)))
            .Verifiable();

        // Act
        await _gameService.UpdateGameWithoutDependenciesAsync(inputGame);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for UpdateGameWithoutDependenciesAsync when a game with the given id does not exist in the repository.
    /// Expects KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task UpdateGameWithoutDependenciesAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var inputGame = new Game { Id = 99, Name = "NewName" };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(inputGame.Id))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.UpdateGameWithoutDependenciesAsync(inputGame));
    }

    /// <summary>
    /// Test for GetAllGamesByPublisherNameAsync method with a known publisher name.
    /// Expects to return a list of GameResponse.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPublisherNameAsync_WithKnownName_ReturnsListOfGameResponse()
    {
        // Arrange
        var publisherName = "TestPublisher";
        var testGames = new List<Game>
        {
            new() { Name = "Test Game 1", Publisher = new Publisher { CompanyName = publisherName } },
            new() { Name = "Test Game 2", Publisher = new Publisher { CompanyName = publisherName } },
        };

        _mockRepository
            .Setup(repository => repository.GetByPublisherNameAsync(publisherName))
            .ReturnsAsync(testGames);

        // Act
        var gameResponses = await _gameService.GetAllGamesByPublisherNameAsync(publisherName);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.All(gameResponses, gameResponse => Assert.Contains(gameResponse.Name, testGames.Select(tg => tg.Name)));
    }

    /// <summary>
    /// Test for GetAllGamesByPublisherNameAsync method with a publisher name not exist in the repository.
    /// Expects to return an empty list.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPublisherNameAsync_WithUnknownName_ReturnsEmptyList()
    {
        // Arrange
        var unknownPublisher = "UnknownPublisher";

        _mockRepository
            .Setup(repository => repository.GetByPublisherNameAsync(unknownPublisher))
            .ReturnsAsync(new List<Game>());

        // Act
        var gameResponses = await _gameService.GetAllGamesByPublisherNameAsync(unknownPublisher);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.Empty(gameResponses);
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformTypeAsync method with a known genre ID.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformTypeAsync_WithKnownId_ReturnsListOfGameResponse()
    {
        // Arrange
        var genreId = 1;
        var testGames = new List<Game>
        {
            new() { Name = "Test Game 1", Genres = new List<Genre> { new() { Id = genreId } } },
            new() { Name = "Test Game 2", Genres = new List<Genre> { new() { Id = genreId } } },
        };

        _mockRepository
            .Setup(repository => repository.GetByGenreIdAsync(genreId))
            .ReturnsAsync(testGames);

        // Act
        var gameResponses = await _gameService.GetAllGamesByPlatformTypeAsync(genreId);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.All(gameResponses, gameResponse => Assert.Contains(gameResponse.Name, testGames.Select(tg => tg.Name)));
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformTypeAsync method with a genre ID that doesn't exist in the repository.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformTypeAsync_WithUnknownId_ReturnsEmptyList()
    {
        // Arrange
        var unknownGenreId = 99;

        _mockRepository
            .Setup(repository => repository.GetByGenreIdAsync(unknownGenreId))
            .ReturnsAsync(new List<Game>());

        // Act
        var gameResponses = await _gameService.GetAllGamesByPlatformTypeAsync(unknownGenreId);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.Empty(gameResponses);
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformTypeAsync method with a known platform type.
    /// Expects to return a list of GameResponse.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformTypeAsync_WithKnownType_ReturnsListOfGameResponse()
    {
        // Arrange
        var platformType = "TestType";
        var testGames = new List<Game>
        {
            new() { Name = "Test Game 1", Platforms = new List<Platform> { new() { Type = platformType } } },
            new() { Name = "Test Game 2", Platforms = new List<Platform> { new() { Type = platformType } } },
        };

        _mockRepository
            .Setup(repository => repository.GetByPlatformTypeAsync(platformType))
            .ReturnsAsync(testGames);

        // Act
        var gameResponses = await _gameService.GetAllGamesByPlatformTypeAsync(platformType);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.All(gameResponses, gameResponse => Assert.Contains(gameResponse.Name, testGames.Select(tg => tg.Name)));
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformTypeAsync method with a platform type not exist in the repository.
    /// Expects to return an empty list.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformTypeAsync_WithUnknownType_ReturnsEmptyList()
    {
        // Arrange
        var unknownPlatformType = "UnknownType";

        _mockRepository
            .Setup(repository => repository.GetByPlatformTypeAsync(unknownPlatformType))
            .ReturnsAsync(new List<Game>());

        // Act
        var gameResponses = await _gameService.GetAllGamesByPlatformTypeAsync(unknownPlatformType);

        // Assert
        Assert.NotNull(gameResponses);
        Assert.Empty(gameResponses);
    }

    /// <summary>
    /// Test for conversion from Game to GameResponse.
    /// </summary>
    [Fact]
    public void FromGame_ConvertsAsExpected()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            GameAlias = "TestAlias",
            Name = "TestName",
            Description = "TestDescription",
            Price = 10,
            UnitInStock = 5,
            Discount = 2,
        };

        // Act
        var gameResponse = GameResponse.FromGame(game);

        // Assert
        Assert.Equal(game.GameAlias, gameResponse.Key);
        Assert.Equal(game.Id.ToString(), gameResponse.Id);
        Assert.Equal(game.Name, gameResponse.Name);
        Assert.Equal(game.Description, gameResponse.Description);
        Assert.Equal(game.Price, gameResponse.Price);
        Assert.Equal(game.UnitInStock, gameResponse.UnitInStock);
        Assert.Equal(game.Discount, gameResponse.Discount);
    }
}
