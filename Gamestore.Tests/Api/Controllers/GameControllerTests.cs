using System.Text;
using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO;
using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Models.Wrappers.Game;
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
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Name = "Valid Name" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };
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
        var invalidGameRequest = new GameWrapper { /* Name is missing */ };
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
        var gameAlias = "TestAlias";
        _gameServiceMock.Setup(x => x.GetGameResponseByAliasAsync(gameAlias)).ReturnsAsync(new GameResponse { Key = gameAlias, Name = "Test Game" });

        // Act
        var result = await _gameController.GetGame(gameAlias);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.UpdateGame"/> method returns an OkResult when ModelState is valid.
    /// </summary>
    [Fact]
    public async Task UpdateGame_ReturnsOkResult_WhenModelStateIsValid()
    {
        // Arrange
        var validGameRequest = new GameWrapper
        {
            GameRequest = new GameRequest { Name = "Valid Name" },
            GenresId = new int[] { 1, 2 },
            PlatformsId = new int[] { 3, 4 },
            PublisherId = 5,
        };
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
        var invalidGameRequest = new GameWrapper { /* Name is missing */ };
        _gameController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _gameController.UpdateGame(invalidGameRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test verifies if the <see cref="GameController.RemoveGame"/> method returns a NoContentResult.
    /// </summary>
    [Fact]
    public async Task RemoveGame_ReturnsNoContentResult()
    {
        // Arrange
        var gameAlias = "TestAlias";

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
        var gameAlias = "TestAlias";
        var game = new GameResponse
        {
            Key = gameAlias,
            Name = "TestGame",
            Description = "TestDescription",
        };
        _gameServiceMock.Setup(x => x.GetGameResponseByAliasAsync(gameAlias)).ReturnsAsync(game);

        // Act
        var result = await _gameController.DownloadGame(gameAlias);

        // Assert
        Assert.IsType<FileContentResult>(result);
        var fileResult = result as FileContentResult;
        Assert.NotNull(fileResult);

        var expectedContent = $"Game Alias: {game.Key}\n" +
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
            new() { Key = "Alias1", Name = "Game1" },
            new() { Key = "Alias2", Name = "Game2" },
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

    /// <summary>
    /// Test for GetGenre method.
    /// </summary>
    [Fact]
    public async Task GetGenre_WhenCalled_CallsServiceWithCorrectIdAndReturnsResult()
    {
        // Arrange
        var gameId = "1";
        var expectedGameResponse = new GameResponse { Id = gameId };

        _gameServiceMock
            .Setup(service => service.GetGameByIdAsync(Convert.ToInt32(gameId)))
            .ReturnsAsync(expectedGameResponse)
            .Verifiable("Service method was not called with correct ID");

        // Act
        var result = await _gameController.GetGenre(gameId);

        // Assert
        _gameServiceMock.Verify();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameResponse>(okResult.Value);
        Assert.Equal(expectedGameResponse, returnValue);
    }

    /// <summary>
    /// Test for GetAllGamesByPublisherName method.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPublisherName_WhenCalled_ReturnsResultFromService()
    {
        // Arrange
        var publisherName = "PublisherName";
        var expectedGames = new List<GameResponse> { new() { Name = "Test Game 1" }, new() { Name = "Test Game 2" } };

        _gameServiceMock
            .Setup(service => service.GetAllGamesByPublisherNameAsync(publisherName))
            .ReturnsAsync(expectedGames);

        // Act
        var result = await _gameController.GetAllGamesByPublisherName(publisherName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualGames = Assert.IsType<List<GameResponse>>(okResult.Value);
        Assert.Equal(expectedGames, actualGames);
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformType method with ID parameter.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformType_WhenCalledWithId_ReturnsResultFromService()
    {
        // Arrange
        var platformTypeId = 1;
        var expectedGames = new List<GameResponse> { new() { Name = "Test Game 1" }, new() { Name = "Test Game 2" } };

        _gameServiceMock
            .Setup(service => service.GetAllGamesByPlatformTypeAsync(platformTypeId))
            .ReturnsAsync(expectedGames);

        // Act
        var result = await _gameController.GetAllGamesByPlatformType(platformTypeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualGames = Assert.IsType<List<GameResponse>>(okResult.Value);
        Assert.Equal(expectedGames, actualGames);
    }

    /// <summary>
    /// Test for GetAllGamesByPlatformType method with platform type parameter.
    /// </summary>
    [Fact]
    public async Task GetAllGamesByPlatformType_WhenCalledWithPlatformType_ReturnsResultFromService()
    {
        // Arrange
        var platformType = "PlatformType";
        var expectedGames = new List<GameResponse> { new() { Name = "Test Game 1" }, new() { Name = "Test Game 2" } };

        _gameServiceMock
            .Setup(service => service.GetAllGamesByPlatformTypeAsync(platformType))
            .ReturnsAsync(expectedGames);

        // Act
        var result = await _gameController.GetAllGamesByPlatformType(platformType);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualGames = Assert.IsType<List<GameResponse>>(okResult.Value);
        Assert.Equal(expectedGames, actualGames);
    }

    /// <summary>
    /// Test to verify that the GetCommentsByGameAlias method of the GameController
    /// calls the GetCommentsByGameAliasAsync method of the GameService.
    /// </summary>
    [Fact]
    public async Task GetCommentsByGameAlias_ShouldCallGetCommentsByGameAliasAsync()
    {
        // Arrange
        var gameAlias = "ExistingGame";

        // Act
        await _gameController.GetCommentsByGameAlias(gameAlias);

        // Assert
        _gameServiceMock.Verify(s => s.GetCommentsByGameAliasAsync(gameAlias), Times.Once);
    }

    /// <summary>
    /// Test to verify that the AddComment method of the GameController
    /// calls the AddCommentAsync and GetCommentsByGameAliasAsync methods of the GameService.
    /// </summary>
    [Fact]
    public async Task AddComment_ShouldCallAddCommentAsyncAndGetCommentsByGameAliasAsync()
    {
        // Arrange
        var gameAlias = "ExistingGame";
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = "Test", Body = "Test body" },
            Action = "Quote",
            ParentId = 1,
        };

        // Act
        await _gameController.AddComment(gameAlias, commentWrapper);

        // Assert
        _gameServiceMock.Verify(s => s.AddCommentAsync(commentWrapper, gameAlias), Times.Once);
        _gameServiceMock.Verify(s => s.GetCommentsByGameAliasAsync(gameAlias), Times.Once);
    }

    /// <summary>
    /// Test to verify that the DeleteComment method of the GameController
    /// calls the DeleteComment and GetCommentsByGameAliasAsync methods of the GameService.
    /// </summary>
    [Fact]
    public async Task DeleteComment_ShouldCallDeleteCommentAndGetCommentsByGameAliasAsync()
    {
        // Arrange
        var gameAlias = "ExistingGame";
        var commentId = 1;

        // Act
        await _gameController.DeleteComment(commentId, gameAlias);

        // Assert
        _gameServiceMock.Verify(s => s.DeleteComment(commentId), Times.Once);
        _gameServiceMock.Verify(s => s.GetCommentsByGameAliasAsync(gameAlias), Times.Once);
    }

    /// <summary>
    /// Test to verify that the GetBanDurations method of the GameController
    /// calls the GetBanDurationsAsync method of the GameService.
    /// </summary>
    [Fact]
    public async Task GetBanDurations_ShouldCallGetBanDurationsAsync()
    {
        // Act
        await _gameController.GetBanDurations();

        // Assert
        _gameServiceMock.Verify(s => s.GetBanDurationsAsync(), Times.Once);
    }

    /// <summary>
    /// Test to verify that the BanUser method of the GameController
    /// calls the BanUserAsync method of the GameService.
    /// </summary>
    [Fact]
    public async Task BanUser_ShouldCallBanUserAsync()
    {
        // Arrange
        var banRequest = new BanRequest { User = "TestUser", Duration = "1 day" };

        // Act
        await _gameController.BanUser(banRequest);

        // Assert
        _gameServiceMock.Verify(s => s.BanUserAsync(banRequest.User, banRequest.Duration), Times.Once);
    }
}
