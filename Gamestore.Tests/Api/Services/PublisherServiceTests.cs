using Gamestore.Api.Models.DTO.PublisherDTO;
using Gamestore.Api.Services;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="PublisherService"/> class.
/// </summary>
public class PublisherServiceTests
{
    private readonly Mock<IPublisherRepository> _mockRepository;
    private readonly IPublisherService _publisherService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherServiceTests"/> class.
    /// Objects for test initialization.
    /// </summary>
    public PublisherServiceTests()
    {
        _mockRepository = new Mock<IPublisherRepository>();
        _publisherService = new PublisherService(_mockRepository.Object);
    }

    /// <summary>
    /// Test for CreatePublisherAsync method when publisher name is unique.
    /// Expects to complete successfully.
    /// </summary>
    [Fact]
    public async Task CreatePublisherAsync_WhenPublisherNameIsUnique_CompletesSuccessfully()
    {
        // Arrange
        var uniqueName = "Unique publisher";
        var publisherRequest = new PublisherRequest
        {
            CompanyName = uniqueName,
            Description = "Description for unique publisher",
            HomePage = "www.uniquepublisher.com",
        };

        _mockRepository
            .Setup(r => r.GetByNameAsync(uniqueName))
            .Returns(Task.FromResult((Publisher)null));

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Publisher>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _publisherService.CreatePublisherAsync(publisherRequest);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for CreatePublisherAsync method when publisher name is not unique.
    /// Expects an InvalidOperationException.
    /// </summary>
    [Fact]
    public void CreatePublisherAsync_WhenPublisherNameIsNotUnique_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonUniqueName = "Non-unique publisher";
        var publisher = new Publisher();

        var publisherRequest = new PublisherRequest
        {
            CompanyName = nonUniqueName,
            Description = "Description for non unique publisher",
            HomePage = "www.nonuniquepublisher.com",
        };

        _mockRepository
            .Setup(r => r.GetByNameAsync(nonUniqueName))
            .ReturnsAsync(publisher);

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _publisherService.CreatePublisherAsync(publisherRequest));
        Assert.NotNull(exception.Result);
        Assert.IsType<InvalidOperationException>(exception.Result);
    }

    /// <summary>
    /// Test for GetPublisherByIdAsync method when the publisher exists.
    /// Expects the publisher details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPublisherByIdAsync_WhenPublisherExists_ReturnsPublisher()
    {
        // Arrange
        var testPublisher = new Publisher { CompanyName = "Test publisher" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(testPublisher);

        // Act
        var publisherResponse = await _publisherService.GetPublisherByIdAsync(It.IsAny<int>());

        // Assert
        Assert.NotNull(publisherResponse);
        Assert.Equal(testPublisher.CompanyName, publisherResponse.CompanyName);
    }

    /// <summary>
    /// Test for GetPublisherByIdAsync method when the publisher does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void GetPublisherByIdAsync_WhenPublisherDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .Returns(Task.FromResult((Publisher)null));

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _publisherService.GetPublisherByIdAsync(It.IsAny<int>()));
        Assert.NotNull(exception.Result);
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for GetPublisherByNameAsync method when the publisher exists.
    /// Expects the publisher details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPublisherByNameAsync_WhenPublisherExists_ReturnsPublisher()
    {
        // Arrange
        var testPublisher = new Publisher { CompanyName = "Test publisher" };

        _mockRepository
            .Setup(r => r.GetByNameAsync(testPublisher.CompanyName))
            .ReturnsAsync(testPublisher);

        // Act
        var publisherResponse = await _publisherService.GetPublisherByNameAsync(testPublisher.CompanyName);

        // Assert
        Assert.NotNull(publisherResponse);
        Assert.Equal(testPublisher.CompanyName, publisherResponse.CompanyName);
    }

    /// <summary>
    /// Test for GetPublisherByNameAsync method when the publisher does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void GetPublisherByNameAsync_WhenPublisherDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistentPublisher = "Non-existent publisher";

        _mockRepository
            .Setup(r => r.GetByNameAsync(nonExistentPublisher))
            .Returns(Task.FromResult((Publisher)null));

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _publisherService.GetPublisherByNameAsync(nonExistentPublisher));
        Assert.NotNull(exception.Result);
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for UpdatePublisherAsync method when the publisher exists.
    /// Expects publisher to be updated successfully.
    /// </summary>
    [Fact]
    public async Task UpdatePublisherAsync_WhenPublisherExists_UpdatesPublisher()
    {
        // Arrange
        var testPublisher = new Publisher { CompanyName = "Test publisher" };

        var publisherRequest = new PublisherRequest
        {
            CompanyName = "Test publisher",
            Description = "Updated description",
            HomePage = "www.updatedpublisher.com",
        };

        _mockRepository
            .Setup(r => r.GetByNameAsync(publisherRequest.CompanyName))
            .ReturnsAsync(testPublisher);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Publisher>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _publisherService.UpdatePublisherAsync(publisherRequest);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for UpdatePublisherAsync method when the publisher does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void UpdatePublisherAsync_WhenPublisherDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((Publisher)null));

        var publisherRequest = new PublisherRequest
        {
            CompanyName = "Non-existent publisher",
            Description = "Description",
            HomePage = "www.nonexistentpublisher.com",
        };

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _publisherService.UpdatePublisherAsync(publisherRequest));
        Assert.NotNull(exception.Result);
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for RemovePublisherAsync method.
    /// Expects publisher to be removed.
    /// </summary>
    [Fact]
    public async Task RemovePublisherAsync_ExecutesSuccessfully()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.RemoveAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _publisherService.RemovePublisherAsync(It.IsAny<int>());

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for GetAllPublishersAsync. Expects to return all publishers.
    /// </summary>
    [Fact]
    public async Task GetAllPublishersAsync_ReturnsAllPublishers()
    {
        // Arrange
        var testPublishers = new List<Publisher>
        {
            new() { CompanyName = "Test Publisher 1" },
            new() { CompanyName = "Test Publisher 2" },
        };

        _mockRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(testPublishers);

        // Act
        var publisherResponses = await _publisherService.GetAllPublishersAsync();

        // Assert
        Assert.NotNull(publisherResponses);
        Assert.All(publisherResponses, publisherResponse => Assert.Contains(publisherResponse.CompanyName, testPublishers.Select(tp => tp.CompanyName)));
    }

    /// <summary>
    /// Test for GetPublishersByGameAliasAsync when the publisher exists.
    /// Expects the publisher details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPublishersByGameAliasAsync_WhenPublishersExist_ReturnsPublishers()
    {
        // Arrange
        var testPublishers = new List<Publisher> { new() { CompanyName = "Test publisher 1" }, new() { CompanyName = "Test publisher 2" } };

        _mockRepository
            .Setup(r => r.GetByGameAliasAsync(It.IsAny<string>()))
            .ReturnsAsync(testPublishers);

        // Act
        var publisherResponses = await _publisherService.GetPublishersByGameAliasAsync(It.IsAny<string>());

        // Assert
        Assert.NotNull(publisherResponses);
        Assert.Equal(testPublishers.Count, publisherResponses.Count());
        Assert.All(publisherResponses, publisherResponse => Assert.Contains(publisherResponse.CompanyName, testPublishers.Select(tp => tp.CompanyName)));
    }
}
