using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO.GenreDTO;
using Gamestore.Api.Models.Wrappers.Genre;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gamestore.Tests.Api.Controllers;

/// <summary>
/// Represents the test class for GenreController. Contains all unit tests for the controller methods.
/// </summary>
public class GenreControllerTests : IDisposable
{
    private readonly Mock<IGenreService> _genreService;
    private readonly GenreController _controller;
    private readonly GenreResponse _testGenreResponse;
    private readonly GenreUpdateRequest _testGenreUpdateRequest;
    private readonly GenreRequest _testGenreRequest;
    private readonly List<GenreResponse> _testGenresList;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreControllerTests"/> class.
    /// Sets up test data and mock dependencies for unit tests.
    /// </summary>
    public GenreControllerTests()
    {
        _genreService = new Mock<IGenreService>();
        _controller = new GenreController(_genreService.Object);

        _testGenreResponse = new GenreResponse { Id = 1, Name = "Test genre" };
        _testGenreUpdateRequest = new GenreUpdateRequest { Id = 1, Name = "Test genre" };
        _testGenreRequest = new GenreRequest { Name = "Test genre" };
        _testGenresList = new List<GenreResponse> { _testGenreResponse };
    }

    /// <summary>
    /// Test method for GetGenre() when a valid id is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetGenre_WhenCalledWithValidId_ReturnsOkObjectResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.GetGenreByIdAsync(It.IsAny<int>())).ReturnsAsync(_testGenreResponse);

        // Act
        var result = await _controller.GetGenre(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GenreResponse>(okResult.Value);
        Assert.Equal(_testGenreResponse, returnValue);
    }

    /// <summary>
    /// Test method for AddGenre() when a valid genre is being added.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task AddGenre_WhenCalledWithValidGenre_ReturnsOkResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.AddGenreAsync(It.IsAny<GenreRequest>()));

        // Act
        var genreWrapper = new GenreAddWrapper { GenreRequest = _testGenreRequest };
        var result = await _controller.AddGenre(genreWrapper);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for UpdateGenre() when a valid genre object is used.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task UpdateGenre_WhenCalledWithValidGenre_ReturnsOkResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.UpdateGenreAsync(It.IsAny<GenreUpdateRequest>()));

        // Act
        var result = await _controller.UpdateGenre(new GenreUpdateWrapper { GenreRequest = _testGenreUpdateRequest });

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for UpdateGenre() when an invalid genre object is used.
    /// The test is considered successful if it results in a BadRequestObjectResult.
    /// </summary>
    [Fact]
    public async Task UpdateGenre_WhenCalledWithInvalidGenre_ReturnsBadRequestObjectResult()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Model error");

        // Act
        var result = await _controller.UpdateGenre(new GenreUpdateWrapper());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test method for RemoveGenre() when a valid id is used.
    /// The test is considered successful if it results in a NoContentResult.
    /// </summary>
    [Fact]
    public async Task RemoveGenre_WhenCalledWithValidId_ReturnsNoContentResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.RemoveGenreAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.RemoveGenre(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Test method for InitPredefinedGenres() when called.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task InitPredefinedGenres_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.InitializePredefinedGenresAsync());

        // Act
        var result = await _controller.InitPredefinedGenres();

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for GetGenresByParentId() when a valid parent genre id is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetGenresByParentId_WhenCalledWithValidGenreParentId_ReturnsOkObjectResult()
    {
        // Arrange
        var testGenres = new List<Genre> { new() { Id = 1, Name = "Test genre" } };
        _genreService.Setup(gs => gs.GetGenresByParentIdAsync(It.IsAny<int>())).ReturnsAsync(testGenres);

        // Act
        var result = await _controller.GetGenresByParentId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<ICollection<Genre>>(okResult.Value);
        Assert.Equal(testGenres, returnValue);
    }

    /// <summary>
    /// Test method for GetAllGenres() when called.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllGenres_WhenCalled_ReturnsOkObjectResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.GetAllGenresAsync()).ReturnsAsync(_testGenresList);

        // Act
        var result = await _controller.GetAllGenres();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<GenreResponse>>(okResult.Value);
        Assert.Equal(_testGenresList, returnValue);
    }

    /// <summary>
    /// Test method for GetAllGenresByGameAlias() when a valid game alias is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllGenresByGameAlias_WhenCalledWithGameAlias_ReturnsOkObjectResult()
    {
        // Arrange
        _genreService.Setup(gs => gs.GetGenresByGameAliasAsync(It.IsAny<string>())).ReturnsAsync(_testGenresList);

        // Act
        var result = await _controller.GetAllGenresByGameAlias("alias");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<GenreResponse>>(okResult.Value);
        Assert.Equal(_testGenresList, returnValue);
    }

    /// <summary>
    /// Cleans up after each unit test has run.
    /// Verifies all expectations on the mock dependencies and frees resources for garbage collection.
    /// </summary>
    public void Dispose()
    {
        _genreService.VerifyAll();
        GC.SuppressFinalize(this);
    }
}
