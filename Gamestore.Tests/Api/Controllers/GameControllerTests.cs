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

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerTests"/> class.
    /// </summary>
    public GameControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _gameController = new GameController(_gameServiceMock.Object);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.CreateGame"/> method returns an OkResult when provided with valid data.
    /// </summary>
    [Fact]
    public async Task CreateGame_ReturnsOkResult_WhenModelStateIsValid()
    {
        // Arrange
        var validGameRequest = new GameRequest { Name = "Valid Name" };
        _gameController.ModelState.Clear();

        // Act
        var result = await _gameController.CreateGame(validGameRequest);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.CreateGame"/> method returns a BadRequestResult when the "Name" is missing.
    /// </summary>
    [Fact]
    public async Task CreateGame_ReturnsBadRequestResult_WhenNameIsMissing()
    {
        // Arrange
        var invalidGameRequest = new GameRequest { /* Name is missing */ };
        _gameController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _gameController.CreateGame(invalidGameRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.GetGame"/> method returns an OkResult when the game exists.
    /// </summary>
    [Fact]
    public async Task GetGame_ReturnsOkResult_WhenGameExists()
    {
        // Arrange
        string gameAlias = "TestAlias";
        _gameServiceMock.Setup(x => x.GetGameByAliasAsync(gameAlias)).ReturnsAsync(new GameResponse { GameAlias = gameAlias, Name = "Test Game" });

        // Act
        var result = await _gameController.GetGame(gameAlias);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.UpdateGame"/> method returns an OkResult when ModelState is valid.
    /// </summary>
    [Fact]
    public async Task UpdateGame_ReturnsOkResult_WhenModelStateIsValid()
    {
        // Arrange
        var validGameRequest = new GameRequest { Name = "Valid Name" };
        _gameController.ModelState.Clear();

        // Act
        var result = await _gameController.UpdateGame(validGameRequest);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.UpdateGame"/> method returns a BadRequestResult when ModelState is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateGame_ReturnsBadRequestResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var invalidGameRequest = new GameRequest { /* Name is missing */ };
        _gameController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _gameController.UpdateGame(invalidGameRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.UpdateGame"/> method calls <see cref="IGameService.UpdateGameAsync"/> when ModelState is valid.
    /// </summary>
    [Fact]
    public async Task UpdateGame_CallsUpdateGameAsync_WhenModelStateIsValid()
    {
        // Arrange
        var validGameRequest = new GameRequest { Name = "Valid Name" };
        _gameController.ModelState.Clear();

        // Act
        await _gameController.UpdateGame(validGameRequest);

        // Assert
        _gameServiceMock.Verify(x => x.UpdateGameAsync(validGameRequest), Times.Once);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.RemoveGame"/> method returns a NoContentResult.
    /// </summary>
    [Fact]
    public async Task RemoveGame_ReturnsNoContentResult()
    {
        // Arrange
        string gameAlias = "TestAlias";

        // Act
        var result = await _gameController.RemoveGame(gameAlias);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _gameServiceMock.Verify(x => x.RemoveGameAsync(gameAlias), Times.Once);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.DownloadGame"/> method returns a FileContentResult with correct content.
    /// </summary>
    [Fact]
    public async Task DownloadGame_ReturnsFileResult_WithCorrectContent()
    {
        // Arrange
        string gameAlias = "TestAlias";
        var game = new GameResponse
        {
            GameAlias = gameAlias,
            Name = "TestGame",
            Description = "TestDescription",
        };
        _gameServiceMock.Setup(x => x.GetGameByAliasAsync(gameAlias)).ReturnsAsync(game);

        // Act
        var result = await _gameController.DownloadGame(gameAlias);

        // Assert
        Assert.IsType<FileContentResult>(result);
        var fileResult = result as FileContentResult;
        Assert.NotNull(fileResult);

        var expectedContent = $"Game Alias: {game.GameAlias}\n" +
                              $"Name: {game.Name}\n" +
                              $"Description: {game.Description}";
        var expectedFileName = $"{game.Name}_{DateTime.Now:yyyyMMddHHmmss}.txt";

        Assert.Equal(expectedContent, Encoding.UTF8.GetString(fileResult.FileContents));
        Assert.Equal("text/plain", fileResult.ContentType);
        Assert.StartsWith(expectedFileName, fileResult.FileDownloadName);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.GetAllGames"/> method returns an OkResult with a list of games.
    /// </summary>
    [Fact]
    public async Task GetAllGames_ReturnsOkResult_WithGamesList()
    {
        // Arrange
        var expectedGames = new List<GameResponse>
        {
            new GameResponse { GameAlias = "Alias1", Name = "Game1" },
            new GameResponse { GameAlias = "Alias2", Name = "Game2" },
        };
        _gameServiceMock.Setup(x => x.GetAllGamesAsync()).ReturnsAsync(expectedGames);

        // Act
        var result = await _gameController.GetAllGames();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);

        var actualGames = okResult.Value as List<GameResponse>;
        Assert.NotNull(actualGames);
        Assert.Equal(expectedGames, actualGames);
    }
}
