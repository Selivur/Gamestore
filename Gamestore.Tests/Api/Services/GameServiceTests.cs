using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Game;
using Gamestore.Api.Services;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;
public class GameServiceTests
{
    private readonly Mock<IGameRepository> _mockRepository;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _mockRepository = new Mock<IGameRepository>();
        _gameService = new GameService(_mockRepository.Object);
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

        // Use a flag to indicate whether AddAsync was called
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
        var result = await _gameService.GetGameByAliasAsync(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingGame.GameAlias, result.Key);
        Assert.Equal(existingGame.Name, result.Name);
        Assert.Equal(existingGame.Description, result.Description);
    }

    [Fact]
    public async Task GetGameByAliasAsync_ShouldThrowException_WhenGameNotFound()
    {
        // Arrange
        var gameAlias = "non-existing-game";

        _mockRepository.Setup(repo => repo.GetByAliasAsync(gameAlias))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByAliasAsync(gameAlias));
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
            Id = 1, // Ensure this matches the Id in GameWrapper
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
}
