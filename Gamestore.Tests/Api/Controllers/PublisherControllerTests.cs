using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO.PublisherDTO;
using Gamestore.Api.Models.Wrappers.Publisher;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gamestore.Tests.Api.Controllers;

/// <summary>
/// Represents test class for PublisherController.
/// </summary>
public class PublisherControllerTests : IDisposable
{
    private readonly Mock<IPublisherService> _publisherService;
    private readonly PublisherController _controller;
    private readonly PublisherResponse _testPublisherResponse;
    private readonly PublisherWrapper _testPublisherWrapper;
    private readonly List<PublisherResponse> _testPublisherList;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherControllerTests"/> class.
    /// Sets up test data for each unit test.
    /// Initializes Mock service, test controller, and test data.
    /// </summary>
    public PublisherControllerTests()
    {
        _publisherService = new Mock<IPublisherService>();
        _controller = new PublisherController(_publisherService.Object);

        _testPublisherResponse = new PublisherResponse
        {
            Id = "1",
            CompanyName = "TestCompany",
            Description = "TestDescription",
            HomePage = "www.test.com",
        };
        _testPublisherWrapper = new()
        {
            PublisherRequest = new()
            {
                CompanyName = "TestCompany",
                Description = "TestDescription",
                HomePage = "www.test.com",
            },
        };
        _testPublisherList = new List<PublisherResponse> { _testPublisherResponse };
    }

    /// <summary>
    /// Test method for GetPublisher() when a valid id is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetPublisher_WhenCalledWithValidId_ReturnsOkObjectResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.GetPublisherByIdAsync(It.IsAny<int>())).ReturnsAsync(_testPublisherResponse);

        // Act
        var result = await _controller.GetPublisher(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PublisherResponse>(okResult.Value);
        Assert.Equal(_testPublisherResponse, returnValue);
    }

    /// <summary>
    /// Test method for CreatePublisher() when a valid publisher object is used.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task CreatePublisher_WhenCalledWithValidPublisher_ReturnsOkResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.CreatePublisherAsync(It.IsAny<PublisherRequest>()));

        // Act
        var result = await _controller.CreatePublisher(_testPublisherWrapper);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for GetPublisher() when a valid publisher name is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetPublisher_WhenCalledWithValidPublisherName_ReturnsOkObjectResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.GetPublisherByNameAsync(It.IsAny<string>())).ReturnsAsync(_testPublisherResponse);

        // Act
        var result = await _controller.GetPublisher(_testPublisherResponse.CompanyName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PublisherResponse>(okResult.Value);
        Assert.Equal(_testPublisherResponse, returnValue);
    }

    /// <summary>
    /// Test method for UpdatePublisher() when a valid publisher object is used.
    /// The test is considered successful if it results in an OkResult.
    /// </summary>
    [Fact]
    public async Task UpdatePublisher_WhenCalledWithValidPublisher_ReturnsOkResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.UpdatePublisherAsync(It.IsAny<PublisherRequest>()));

        // Act
        var result = await _controller.UpdatePublisher(_testPublisherWrapper);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test method for RemovePublisher() when a valid id is used.
    /// The test is considered successful if it results in a NoContentResult.
    /// </summary>
    [Fact]
    public async Task RemovePublisher_WhenCalledWithValidId_ReturnsNoContentResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.RemovePublisherAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.RemovePublisher(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Test method for GetAllPublishers() when called.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllPublishers_WhenCalled_ReturnsOkObjectResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.GetAllPublishersAsync()).ReturnsAsync(_testPublisherList);

        // Act
        var result = await _controller.GetAllPublishers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<PublisherResponse>>(okResult.Value);
        Assert.Equal(_testPublisherList, returnValue);
    }

    /// <summary>
    /// Test method for GetAllPublishersByGameAlias() when a valid game alias is used.
    /// The test is considered successful if it results in an OkObjectResult.
    /// </summary>
    [Fact]
    public async Task GetAllPublishersByGameAlias_WhenCalledWithValidGameAlias_ReturnsOkObjectResult()
    {
        // Arrange
        _publisherService.Setup(ps => ps.GetPublishersByGameAliasAsync(It.IsAny<string>())).ReturnsAsync(_testPublisherList);
        var validGameAlias = "game-alias";

        // Act
        var result = await _controller.GetAllPublishersByGameAlias(validGameAlias);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<PublisherResponse>>(okResult.Value);
        Assert.Equal(_testPublisherList, returnValue);
    }

    /// <summary>
    /// Test method for CreatePublisher() when an invalid publisher is used.
    /// The test is considered successful if it results in a BadRequestObjectResult.
    /// </summary>
    [Fact]
    public async Task CreatePublisher_WhenCalledWithInvalidPublisher_ReturnsBadRequestObjectResult()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Model error");

        // Act
        var result = await _controller.CreatePublisher(new PublisherWrapper());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test method for UpdatePublisher() when an invalid publisher is used.
    /// The test is considered successful if it results in a BadRequestObjectResult.
    /// </summary>
    [Fact]
    public async Task UpdatePublisher_WhenCalledWithInvalidPublisher_ReturnsBadRequestObjectResult()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Model error");

        // Act
        var result = await _controller.UpdatePublisher(new PublisherWrapper());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Invoked after each unit test has run.
    /// Verifies all verifiable expectations on this mock. Invocation is cleared after verification.
    /// Suppresses finalize for the current instance from garbage collector's queue to prevent the finalizer code for
    /// this object from executing a second time.
    /// </summary>
    public void Dispose()
    {
        _publisherService.VerifyAll();

        GC.SuppressFinalize(this);
    }
}
