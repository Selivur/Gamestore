using System.Text;
using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gamestore.Tests.Api.Controllers;
public class GameControllerTests
{
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly GameController _gameController;

    public GameControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _gameController = new GameController(_gameServiceMock.Object);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.CreateGame"/> method returns an OkResult when provided with valid data.
    /// </summary>
    [Fact]
    public async Task CreateGame_WhenModelStateIsValid_ShouldReturnOkResult()
    {
        // Arrange
        _gameServiceMock.Setup(s => s.CreateGameAsync(It.IsAny<GameRequest>()))
            .ReturnsAsync((true, null));
        var gameRequest = new GameRequest { GameAlias = "new-game", Name = "New Game", Description = "A new game" };

        // Act
        var result = await _gameController.CreateGame(gameRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<GameRequest>(okResult.Value);
        Assert.Equal(gameRequest, model);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.CreateGame"/> method returns a BadRequest with a list of errors in ModelState when ModelState is not valid.
    /// </summary>
    [Fact]
    public async Task CreateGame_WhenModelStateIsNotValid_ShouldReturnBadRequestWithErrors()
    {
        // Arrange
        _gameController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _gameController.CreateGame(new GameRequest());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessages = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Single(errorMessages);
        Assert.Equal("Name is required", errorMessages[0]);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.CreateGame"/> method returns a BadRequest with an error message when the game creation service fails.
    /// </summary>
    [Fact]
    public async Task CreateGame_WhenGameServiceFails_ShouldReturnBadRequestWithErrorMessage()
    {
        // Arrange
        _gameServiceMock.Setup(s => s.CreateGameAsync(It.IsAny<GameRequest>()))
            .ReturnsAsync((false, "An error occurred"));

        var gameRequest = new GameRequest { GameAlias = "new-game", Name = "New Game", Description = "A new game" };

        // Act
        var result = await _gameController.CreateGame(gameRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("An error occurred", errorMessage);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.GetGame"/> method returns an OkResult with a game when a valid game alias is provided.
    /// </summary>
    [Fact]
    public async Task GetGame_WithValidAlias_ShouldReturnOkResultWithGame()
    {
        // Arrange
        var gameAlias = "valid-game";
        var gameResponse = new GameResponse { GameAlias = gameAlias, Name = "Valid Game" };
        _gameServiceMock.Setup(s => s.GetGameByAliasAsync(gameAlias)).ReturnsAsync((true, null, gameResponse));

        // Act
        var result = await _gameController.GetGame(gameAlias);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<GameResponse>(okResult.Value);
        Assert.Equal(gameAlias, returnedGame.GameAlias);
        Assert.Equal("Valid Game", returnedGame.Name);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.GetGame"/> method returns a BadRequest result with an error message when an invalid game alias is provided.
    /// </summary>
    [Fact]
    public async Task GetGame_WithInvalidAlias_ShouldReturnBadRequestWithErrorMessage()
    {
        // Arrange
        var invalidGameAlias = "invalid-game";
        var errorMessage = "Game not found";
        _gameServiceMock.Setup(s => s.GetGameByAliasAsync(invalidGameAlias)).ReturnsAsync((false, errorMessage, null));

        // Act
        var result = await _gameController.GetGame(invalidGameAlias);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedMessage = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(errorMessage, returnedMessage);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.GetGame"/> method returns a BadRequest result with an error message when the game retrieval service fails.
    /// </summary>
    [Fact]
    public async Task GetGame_WhenGameServiceFails_ShouldReturnBadRequestWithErrorMessage()
    {
        // Arrange
        var gameAlias = "valid-game";
        var errorMessage = "Game retrieval service failed";
        _gameServiceMock.Setup(s => s.GetGameByAliasAsync(gameAlias)).ReturnsAsync((false, errorMessage, null));

        // Act
        var result = await _gameController.GetGame(gameAlias);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedMessage = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(errorMessage, returnedMessage);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.UpdateGame"/> method returns an OK result with the updated game when the game is successfully updated.
    /// </summary>
    [Fact]
    public async Task UpdateGame_WhenGameIsUpdated_ShouldReturnOkResultWithUpdatedGame()
    {
        // Arrange
        var updatedGame = new GameRequest
        {
            GameAlias = "updated-game",
            Name = "Updated Game",
            Description = "An updated game",
        };
        _gameServiceMock.Setup(s => s.UpdateGameAsync(updatedGame)).ReturnsAsync((true, null));

        // Act
        var result = await _gameController.UpdateGame(updatedGame);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<GameRequest>(okResult.Value);
        Assert.Equal(updatedGame.GameAlias, returnedGame.GameAlias);
        Assert.Equal("Updated Game", returnedGame.Name);
        Assert.Equal("An updated game", returnedGame.Description);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.UpdateGame"/> method returns a Bad Request result with error messages when the model state is not valid.
    /// </summary>
    [Fact]
    public async Task UpdateGame_WhenModelStateIsNotValid_ShouldReturnBadRequestWithErrors()
    {
        // Arrange
        var invalidGame = new GameRequest
        {
            GameAlias = "invalid-game",
        };
        var errorMessage = "Invalid game data";

        _gameController.ModelState.AddModelError("Name", errorMessage);

        // Act
        var result = await _gameController.UpdateGame(invalidGame);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedMessages = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Contains(errorMessage, returnedMessages);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.UpdateGame"/> method returns a Bad Request result with an error message when the game service fails to update the game.
    /// </summary>
    [Fact]
    public async Task UpdateGame_WhenGameServiceFails_ShouldReturnBadRequestWithErrorMessage()
    {
        // Arrange
        var failedGame = new GameRequest
        {
            GameAlias = "failed-game",
            Name = "Failed Game",
        };
        var errorMessage = "Game update service failed";
        _gameServiceMock.Setup(s => s.UpdateGameAsync(failedGame)).ReturnsAsync((false, errorMessage));

        // Act
        var result = await _gameController.UpdateGame(failedGame);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedMessage = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(errorMessage, returnedMessage);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.RemoveGame"/> method returns a No Content result when the game to remove exists and is successfully removed.
    /// </summary>
    [Fact]
    public async Task RemoveGame_WhenGameIsRemoved_ShouldReturnNoContent()
    {
        // Arrange
        var gameAlias = "existing-game";
        _gameServiceMock.Setup(s => s.RemoveGameAsync(gameAlias)).ReturnsAsync((true, null));

        // Act
        var result = await _gameController.RemoveGame(gameAlias);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.RemoveGame"/> method returns a Not Found result with an error message when the game to remove does not exist.
    /// </summary>
    [Fact]
    public async Task RemoveGame_WhenGameIsNotRemoved_ShouldReturnNotFound()
    {
        // Arrange
        var gameAlias = "non-existing-game";
        var errorMessage = "Game not found";
        _gameServiceMock.Setup(s => s.RemoveGameAsync(gameAlias)).ReturnsAsync((false, errorMessage));

        // Act
        var result = await _gameController.RemoveGame(gameAlias);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnedMessage = Assert.IsType<string>(notFoundResult.Value);
        Assert.Equal(errorMessage, returnedMessage);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.DownloadGame"/> method returns a file content result with the correct content, content type, and file name.
    /// </summary>
    [Fact]
    public async Task DownloadGame_WhenGameIsDownloaded_ReturnsFileContent()
    {
        // Arrange
        var gameAlias = "game-alias";
        var game = new GameResponse
        {
            GameAlias = gameAlias,
            Name = "Game Name",
            Description = "Game Description",
        };
        _gameServiceMock.Setup(x => x.GetGameByAliasAsync(gameAlias))
            .ReturnsAsync((true, null, game));

        // Act
        var result = await _gameController.DownloadGame(gameAlias);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/plain", fileResult.ContentType);
        Assert.Equal($"{game.Name}_{DateTime.Now:yyyyMMddHHmmss}.txt", fileResult.FileDownloadName);
        var expectedContent = $"Game Alias: {game.GameAlias}\n" +
                              $"Name: {game.Name}\n" +
                              $"Description: {game.Description}";
        var actualContent = Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Equal(expectedContent, actualContent);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.DownloadGame"/> method returns a not found object result with the correct error message when the game service returns an error.
    /// </summary>
    [Fact]
    public async Task DownloadGame_WhenGameIsNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var gameAlias = "non-existent-game";
        _gameServiceMock.Setup(s => s.GetGameByAliasAsync(gameAlias)).ReturnsAsync((false, "Game not found", null));

        // Act
        var result = await _gameController.DownloadGame(gameAlias) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Game not found", result.Value);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.GetAllGames"/> method returns an OK result with a list of games when games exist.
    /// </summary>
    [Fact]
    public async Task GetAllGames_WhenGamesExist_ShouldReturnOkResultWithGames()
    {
        // Arrange
        var games = new List<GameResponse>
        {
            new GameResponse { GameAlias = "game1", Name = "Game 1" },
            new GameResponse { GameAlias = "game2", Name = "Game 2" },
        };

        _gameServiceMock.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);

        // Act
        var result = await _gameController.GetAllGames() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var gameResponses = Assert.IsType<List<GameResponse>>(result.Value);
        Assert.Equal(2, gameResponses.Count);
    }

    /// <summary>
    /// Tests that the <see cref="GameController.GetAllGames"/> method returns a No Content result when no games exist.
    /// </summary>
    [Fact]
    public async Task GetAllGames_WhenNoGamesExist_ShouldReturnNoContent()
    {
        // Arrange
        _gameServiceMock.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(new List<GameResponse>());

        // Act
        var result = await _gameController.GetAllGames() as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }
}
