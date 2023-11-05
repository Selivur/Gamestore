using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Services;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;
public class GameServiceTests
{
    /// <summary>
    /// Positive test that verifies whether the CreateGameAsync method of the GameService correctly
    /// creates a new game when the provided game alias is unique.
    /// </summary>
    [Fact]
    public async Task CreateGameAsync_ShouldCreateGame()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "new-game",
            Name = "New Game",
            Description = "A new game",
        };

        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).ReturnsAsync((Game)null);

        // Act
        var (isSuccess, errorMessage) = await service.CreateGameAsync(gameRequest);

        // Assert
        Assert.True(isSuccess);
        Assert.Null(errorMessage);

        repository.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
    }

    /// <summary>
    /// Positive test that verifies whether the CreateGameAsync method of the GameService
    /// correctly returns an error message when the provided game alias is not unique.
    /// </summary>
    [Fact]
    public async Task CreateGameAsync_ShouldReturnErrorIfGameAliasNotUnique()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "existing-game",
            Name = "Existing Game",
            Description = "An existing game",
        };

        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).ReturnsAsync(new Game());

        // Act
        var (isSuccess, errorMessage) = await service.CreateGameAsync(gameRequest);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("Game alias must be unique", errorMessage);

        repository.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
    }

    /// <summary>
    /// Test that checks whether the CreateGameAsync method of the GameService correctly
    /// handles an exception during execution and returns the appropriate error message.
    /// </summary>
    [Fact]
    public async Task CreateGameAsync_ShouldHandleException()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).Throws(new Exception("Test exception"));
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "new-game",
            Name = "New Game",
            Description = "A new game",
        };

        // Act
        var (isSuccess, errorMessage) = await service.CreateGameAsync(gameRequest);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("An error occurred: Test exception", errorMessage);

        repository.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
    }

    /// <summary>
    /// Positive test that verifies whether the GetGameByAliasAsync method of the GameService
    /// correctly returns a game with the specified alias when such a game exists in the repository.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_ShouldReturnGame()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameAlias = "existing-game";
        var existingGame = new Game
        {
            GameAlias = gameAlias,
            Name = "Existing Game",
            Description = "An existing game",
        };

        repository.Setup(r => r.GetByAliasAsync(gameAlias)).ReturnsAsync(existingGame);

        // Act
        var result = await service.GetGameByAliasAsync(gameAlias);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
        Assert.NotNull(result.Game);
        Assert.Equal(gameAlias, result.Game.GameAlias);
        Assert.Equal(existingGame.Name, result.Game.Name);
        Assert.Equal(existingGame.Description, result.Game.Description);
    }

    /// <summary>
    /// Positive test that verifies whether the GetGameByAliasAsync method of the GameService
    /// correctly returns a response indicating that the game with the specified alias is not found
    /// when such a game is absent in the repository.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_ShouldReturnGameNotFound()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameAlias = "non-existent-game";

        repository.Setup(r => r.GetByAliasAsync(gameAlias)).ReturnsAsync((Game)null);

        // Act
        var result = await service.GetGameByAliasAsync(gameAlias);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Game not found", result.ErrorMessage);
        Assert.Null(result.Game);
    }

    /// <summary>
    /// Test that checks whether the GetGameByAliasAsync method of the GameService correctly handles
    /// an exception that occurred during the execution of the repository method and returns the
    /// appropriate error message.
    /// </summary>
    [Fact]
    public async Task GetGameByAliasAsync_ShouldReturnErrorMessageOnException()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameAlias = "existing-game";

        repository.Setup(r => r.GetByAliasAsync(gameAlias)).ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await service.GetGameByAliasAsync(gameAlias);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred", result.ErrorMessage);
        Assert.Null(result.Game);
    }

    /// <summary>
    /// Positive test that verifies whether the UpdateGameAsync method of the
    /// GameService correctly updates an existing game with valid input data.
    /// </summary>
    [Fact]
    public async Task UpdateGameAsync_ShouldUpdateGame()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "existing-game",
            Name = "Updated Game",
            Description = "An updated game",
        };

        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).ReturnsAsync(new Game());

        // Act
        var (isSuccess, errorMessage) = await service.UpdateGameAsync(gameRequest);

        // Assert
        Assert.True(isSuccess);
        Assert.Null(errorMessage);

        repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Once);
    }

    /// <summary>
    /// Positive test that verifies whether the UpdateGameAsync method of the
    /// GameService correctly returns an error message when attempting to update a game with a non-existent alias.
    /// </summary>
    [Fact]
    public async Task UpdateGameAsync_ShouldReturnErrorIfGameNotFound()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "non-existing-game",
            Name = "Updated Game",
            Description = "An updated game",
        };

        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).ReturnsAsync((Game)null);

        // Act
        var (isSuccess, errorMessage) = await service.UpdateGameAsync(gameRequest);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("Can't find the Game with this Alias", errorMessage);

        repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
    }

    /// <summary>
    /// Test that checks whether the UpdateGameAsync method of the
    /// GameService correctly handles an exception during execution and returns the appropriate error message.
    /// </summary>
    [Fact]
    public async Task UpdateGameAsync_ShouldHandleException()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.GetByAliasAsync(It.IsAny<string>())).Throws(new Exception("Test exception"));
        var service = new GameService(repository.Object);

        var gameRequest = new GameRequest
        {
            GameAlias = "existing-game",
            Name = "Updated Game",
            Description = "An updated game",
        };

        // Act
        var (isSuccess, errorMessage) = await service.UpdateGameAsync(gameRequest);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("Test exception", errorMessage);

        repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
    }

    /// <summary>
    /// Positive test that verifies whether the RemoveGameAsync method
    /// of the GameService correctly removes an existing game by its alias.
    /// </summary>
    [Fact]
    public async Task RemoveGameAsync_ShouldRemoveGame()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);

        var gameAlias = "existing-game";

        // Act
        var (isSuccess, errorMessage) = await service.RemoveGameAsync(gameAlias);

        // Assert
        Assert.True(isSuccess);
        Assert.Null(errorMessage);

        repository.Verify(r => r.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    /// Test that checks whether the RemoveGameAsync method of the GameService
    /// correctly handles an exception during execution and returns the appropriate error message.
    /// </summary>
    [Fact]
    public async Task RemoveGameAsync_ShouldHandleException()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.RemoveAsync(It.IsAny<string>())).Throws(new Exception("Test exception"));
        var service = new GameService(repository.Object);

        var gameAlias = "existing-game";

        // Act
        var (isSuccess, errorMessage) = await service.RemoveGameAsync(gameAlias);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal("Test exception", errorMessage);

        repository.Verify(r => r.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    /// Test to verify that the GetAllGamesAsync method of the GameService
    /// retrieves and correctly maps games from the repository to GameResponse objects.
    /// </summary>
    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnGameResponses()
    {
        // Arrange
        var repository = new Mock<IGameRepository>();
        var service = new GameService(repository.Object);
        var games = new List<Game>
        {
            new Game { GameAlias = "game1", Name = "Game 1", Description = "Description 1" },
            new Game { GameAlias = "game2", Name = "Game 2", Description = "Description 2" },
        };

        repository.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

        // Act
        var result = await service.GetAllGamesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(games.Count, result.Count());
    }
}
