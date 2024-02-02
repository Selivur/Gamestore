using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO.PlatformDTO;
using Gamestore.Api.Models.Wrappers.Platform;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gamestore.Tests.Api.Controllers;

/// <summary>
/// Represents test class for PlatformController.
/// Contains all unit tests for the controller methods.
/// </summary>
public class PlatformControllerTests : IDisposable
{
    private readonly Mock<IPlatformService> _platformService;
    private readonly PlatformController _controller;
    private readonly PlatformResponse _testPlatformResponse;
    private readonly PlatformRequest _testPlatformRequest;
    private readonly List<PlatformResponse> _testPlatformList;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformControllerTests"/> class.
    /// Sets up test data and mock dependencies for unit tests.
    /// </summary>
    public PlatformControllerTests()
    {
        _platformService = new Mock<IPlatformService>();
        _controller = new PlatformController(_platformService.Object);

        _testPlatformResponse = new PlatformResponse { Id = 1, Type = "Test platform type" };
        _testPlatformRequest = new PlatformRequest { Id = 1, Type = "Test platform type" };
        _testPlatformList = new List<PlatformResponse> { _testPlatformResponse };
    }

    /// <summary>
    /// Test method for GetPlatform() when a valid id is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetPlatform_WhenCalledWithValidId_ReturnsOkObjectResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.GetPlatformByIdAsync(It.IsAny<int>())).ReturnsAsync(_testPlatformResponse);

        // Act
        var result = await _controller.GetPlatform(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PlatformResponse>(okResult.Value);
        Assert.Equal(_testPlatformResponse, returnValue);
    }

    /// <summary>
    /// Test method for AddPlatform() when a valid platform is being added.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task AddPlatform_WhenCalledWithValidPlatform_ReturnsOkResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.AddPlatformAsync(It.IsAny<string>()));
        var platformWrapper = new PlatformAddWrapper { PlatformTypeRequest = new PlatformTypeRequest { Type = "Test platform type" } };

        // Act
        var result = await _controller.AddPlatform(platformWrapper);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for GetAllPlatforms() when called.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllPlatforms_WhenCalled_ReturnsOkObjectResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.GetAllPlatformsAsync()).ReturnsAsync(_testPlatformList);

        // Act
        var result = await _controller.GetAllPlatforms();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<PlatformResponse>>(okResult.Value);
        Assert.Equal(_testPlatformList, returnValue);
    }

    /// <summary>
    /// Test method for GetAllPlatformsByGameAlias() when a valid game alias is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllPlatformsByGameAlias_WhenCalledWithValidGameAlias_ReturnsOkObjectResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.GetPlatformsByGameAliasAsync(It.IsAny<string>())).ReturnsAsync(_testPlatformList);

        // Act
        var result = await _controller.GetAllPlatformsByGameAlias("alias");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<PlatformResponse>>(okResult.Value);
        Assert.Equal(_testPlatformList, returnValue);
    }

    /// <summary>
    /// Test method for UpdatePlatform() when a valid platform object is used.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task UpdatePlatform_WhenCalledWithValidPlatform_ReturnsOkResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.UpdatePlatformAsync(It.IsAny<PlatformRequest>()));

        // Act
        var result = await _controller.UpdatePlatform(new PlatformUpdateWrapper { PlatformRequest = _testPlatformRequest });

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for RemovePlatform() when a valid id is used.
    /// The test is considered successful if it results in a NoContentResult.
    /// </summary>
    [Fact]
    public async Task RemovePlatform_WhenCalledWithValidId_ReturnsNoContentResult()
    {
        // Arrange
        _platformService.Setup(ps => ps.RemovePlatformAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.RemovePlatform(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Test method for UpdatePlatform() when an invalid platform object is used.
    /// The test is considered successful if it results in a BadRequestObjectResult.
    /// </summary>
    [Fact]
    public async Task UpdatePlatform_WhenCalledWithInvalidPlatform_ReturnsBadRequestObjectResult()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Model error");

        // Act
        var result = await _controller.UpdatePlatform(new PlatformUpdateWrapper());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Cleans up after each unit test has run.
    /// Verifies all expectations on the mock dependencies and frees resources for garbage collection.
    /// </summary>
    public void Dispose()
    {
        _platformService.VerifyAll();
        GC.SuppressFinalize(this);
    }
}
