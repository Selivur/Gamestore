using Gamestore.Api.Models.DTO.PlatformDTO;
using Gamestore.Api.Services;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="PlatformService"/> class.
/// </summary>
public class PlatformServiceTests
{
    private readonly Mock<IPlatformRepository> _mockRepository;
    private readonly IPlatformService _platformService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformServiceTests"/> class.
    /// Setup for tests of the <see cref="PlatformService"/> class.
    /// </summary>
    public PlatformServiceTests()
    {
        _mockRepository = new Mock<IPlatformRepository>();
        _platformService = new PlatformService(_mockRepository.Object);
    }

    /// <summary>
    /// Test for GetAllPlatformsAsync. Expects to return all platforms.
    /// </summary>
    [Fact]
    public async Task GetAllPlatformsAsync_ReturnsAllPlatforms()
    {
        // Arrange
        var testPlatforms = new List<Platform>
        {
            new() { Type = "Test Platform 1" },
            new() { Type = "Test Platform 2" },
        };

        _mockRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(testPlatforms);

        // Act
        var platformResponses = await _platformService.GetAllPlatformsAsync();

        // Assert
        Assert.NotNull(platformResponses);
        Assert.All(platformResponses, platformResponse => Assert.Contains(platformResponse.Type, testPlatforms.Select(tp => tp.Type)));
    }

    /// <summary>
    /// Test for AddPlatformAsync method when platform name is unique.
    /// Expects successful completion.
    /// </summary>
    [Fact]
    public async Task AddPlatformAsync_WhenPlatformNameIsUnique_CompletesSuccessfully()
    {
        // Arrange
        var uniqueName = "Unique platform";

        _mockRepository
            .Setup(r => r.GetByNameAsync(uniqueName))
            .Returns(Task.FromResult((Platform)null));

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Platform>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _platformService.AddPlatformAsync(uniqueName);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for AddPlatformAsync method when platform name is not unique.
    /// Expects an InvalidOperationException.
    /// </summary>
    [Fact]
    public void AddPlatformAsync_WhenPlatformNameIsNotUnique_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonUniqueName = "Non-unique platform";
        var platform = new Platform();

        _mockRepository
            .Setup(r => r.GetByNameAsync(nonUniqueName))
            .ReturnsAsync(platform);

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _platformService.AddPlatformAsync(nonUniqueName));
        Assert.NotNull(exception.Result);
        Assert.IsType<InvalidOperationException>(exception.Result);
    }

    /// <summary>
    /// Test for GetPlatformByNameAsync method when the platform exists.
    /// Expects the platform details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPlatformByNameAsync_WhenPlatformExists_ReturnsPlatform()
    {
        // Arrange
        var testPlatform = new Platform { Type = "Test platform" };

        _mockRepository
            .Setup(r => r.GetByNameAsync(testPlatform.Type))
            .ReturnsAsync(testPlatform);

        // Act
        var platformResponse = await _platformService.GetPlatformByNameAsync(testPlatform.Type);

        // Assert
        Assert.NotNull(platformResponse);
        Assert.Equal(testPlatform.Type, platformResponse.Type);
    }

    /// <summary>
    /// Test for GetPlatformByNameAsync method when the platform does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void GetPlatformByNameAsync_WhenPlatformDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistentPlatform = "Non-existent platform";

        _mockRepository
            .Setup(r => r.GetByNameAsync(nonExistentPlatform))
            .Returns(Task.FromResult((Platform)null));

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _platformService.GetPlatformByNameAsync(nonExistentPlatform));
        Assert.NotNull(exception.Result);
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for GetPlatformByIdAsync method when the platform exists.
    /// Expects the platform details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPlatformByIdAsync_WhenPlatformExists_ReturnsPlatform()
    {
        // Arrange
        var testPlatform = new Platform { Type = "Test platform" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(testPlatform);

        // Act
        var platformResponse = await _platformService.GetPlatformByIdAsync(It.IsAny<int>());

        // Assert
        Assert.NotNull(platformResponse);
        Assert.Equal(testPlatform.Type, platformResponse.Type);
    }

    /// <summary>
    /// Test for GetPlatformByIdAsync method when the platform does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void GetPlatformByIdAsync_WhenPlatformDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .Returns(Task.FromResult((Platform)null));

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _platformService.GetPlatformByIdAsync(It.IsAny<int>()));
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for UpdatePlatformAsync method when the platform exists.
    /// Expects platform to be updated successfully.
    /// </summary>
    [Fact]
    public async Task UpdatePlatformAsync_WhenPlatformExists_UpdatesPlatform()
    {
        // Arrange
        var testPlatform = new Platform { Id = 1, Type = "Test platform" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(testPlatform.Id))
            .ReturnsAsync(testPlatform);

        var platformRequest = new PlatformRequest { Id = testPlatform.Id, Type = "Updated platform" };

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Platform>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _platformService.UpdatePlatformAsync(platformRequest);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for UpdatePlatformAsync method when the platform does not exist.
    /// Expects a KeyNotFoundException.
    /// </summary>
    [Fact]
    public void UpdatePlatformAsync_WhenPlatformDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .Returns(Task.FromResult((Platform)null));

        var platformRequest = new PlatformRequest { Id = 1, Type = "Non-existent platform" };

        // Act and Assert
        var exception = Record.ExceptionAsync(() => _platformService.UpdatePlatformAsync(platformRequest));
        Assert.IsType<KeyNotFoundException>(exception.Result);
    }

    /// <summary>
    /// Test for RemovePlatformAsync method.
    /// Expects platform to be removed.
    /// </summary>
    [Fact]
    public async Task RemovePlatformAsync_ExecutesSuccessfully()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.RemoveAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _platformService.RemovePlatformAsync(It.IsAny<int>());

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for GetPlatformsByGameAliasAsync when the platform exists.
    /// Expects the platform details to be returned.
    /// </summary>
    [Fact]
    public async Task GetPlatformsByGameAliasAsync_WhenPlatformsExist_ReturnsPlatforms()
    {
        // Arrange
        var testPlatforms = new List<Platform> { new() { Type = "Test platform 1" }, new() { Type = "Test platform 2" } };

        _mockRepository
            .Setup(r => r.GetByGameAliasAsync(It.IsAny<string>()))
            .ReturnsAsync(testPlatforms);

        // Act
        var platformResponses = await _platformService.GetPlatformsByGameAliasAsync(It.IsAny<string>());

        // Assert
        Assert.NotNull(platformResponses);
        Assert.Equal(testPlatforms.Count, platformResponses.Count());
        Assert.All(platformResponses, platformResponse => Assert.Contains(platformResponse.Type, testPlatforms.Select(tp => tp.Type)));
    }
}
