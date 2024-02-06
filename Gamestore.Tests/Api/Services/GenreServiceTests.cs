using Gamestore.Api.Models.DTO.GenreDTO;
using Gamestore.Api.Services;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="GenreService"/> class.
/// </summary>
public class GenreServiceTests
{
    private readonly Mock<IGenreRepository> _mockRepository;
    private readonly GenreService _genreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreServiceTests"/> class.
    /// Class constructor setting up the service and the repository mockup.
    /// </summary>
    public GenreServiceTests()
    {
        _mockRepository = new Mock<IGenreRepository>();
        _genreService = new GenreService(_mockRepository.Object);
    }

    /// <summary>
    /// Tests the GetAllGenresAsync method to make sure it returns all genres.
    /// </summary>
    [Fact]
    public async Task GetAllGenresAsync_ReturnsAllGenres()
    {
        // Arrange
        var testGenres = new List<Genre>
        {
            new() { Name = "Test Genre 1" },
            new() { Name = "Test Genre 2" },
        };

        _mockRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(testGenres);

        // Act
        var genreResponses = await _genreService.GetAllGenresAsync();

        // Assert
        Assert.NotNull(genreResponses);
        Assert.All(genreResponses, genreResponse => Assert.Contains(genreResponse.Name, testGenres.Select(tg => tg.Name)));
    }

    /// <summary>
    /// Test for AddGenreAsync when genre does not exist in the repository.
    /// Expects the new genre will be added.
    /// </summary>
    [Fact]
    public async Task AddGenreAsync_WhenGenreDoesNotExists_AddsNewGenre()
    {
        // Arrange
        var genreRequest = new GenreRequest { Name = "NewGenre" };

        _mockRepository
            .Setup(repository => repository.GetByNameAsync(genreRequest.Name))
            .ReturnsAsync((Genre)null);
        _mockRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _genreService.AddGenreAsync(genreRequest);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for AddGenreAsync when genre already exist in the repository.
    /// Expects InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task AddGenreAsync_WhenGenreAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingGenre = new Genre { Name = "ExistingGenre" };
        var genreRequest = new GenreRequest { Name = existingGenre.Name };

        _mockRepository
            .Setup(repository => repository.GetByNameAsync(existingGenre.Name))
            .ReturnsAsync(existingGenre);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _genreService.AddGenreAsync(genreRequest));
    }

    /// <summary>
    /// Test for GetGenreByNameAsync when genre exists in the repository.
    /// Expects to return an equivalent GenreResponse.
    /// </summary>
    [Fact]
    public async Task GetGenreByNameAsync_WithExistingName_ReturnsGenreResponse()
    {
        // Arrange
        var testGenre = new Genre { Name = "ExistingGenre" };

        _mockRepository
            .Setup(repository => repository.GetByNameAsync(testGenre.Name))
            .ReturnsAsync(testGenre);

        // Act
        var genreResponse = await _genreService.GetGenreByNameAsync(testGenre.Name);

        // Assert
        Assert.NotNull(genreResponse);
        Assert.Equal(testGenre.Name, genreResponse.Name);
    }

    /// <summary>
    /// Test for GetGenreByNameAsync when genre does not exist in the repository.
    /// Expects KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task GetGenreByNameAsync_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistentGenreName = "NonExistentGenreName";

        _mockRepository
            .Setup(repository => repository.GetByNameAsync(nonExistentGenreName))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _genreService.GetGenreByNameAsync(nonExistentGenreName));
    }

    /// <summary>
    /// Test for GetGenreByIdAsync when genre exists in the repository.
    /// Expects to return an equivalent GenreResponse.
    /// </summary>
    [Fact]
    public async Task GetGenreByIdAsync_WithExistingId_ReturnsGenreResponse()
    {
        // Arrange
        var testGenre = new Genre { Id = 1, Name = "ExistingGenre" };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(testGenre.Id))
            .ReturnsAsync(testGenre);

        // Act
        var genreResponse = await _genreService.GetGenreByIdAsync(testGenre.Id);

        // Assert
        Assert.NotNull(genreResponse);
        Assert.Equal(testGenre.Name, genreResponse.Name);
    }

    /// <summary>
    /// Test for GetGenreByIdAsync when genre does not exist in the repository.
    /// Expects KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task GetGenreByIdAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistentGenreId = 999;

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(nonExistentGenreId))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _genreService.GetGenreByIdAsync(nonExistentGenreId));
    }

    /// <summary>
    /// Test for UpdateGenreAsync when genre exists and request is valid.
    /// </summary>
    [Fact]
    public async Task UpdateGenreAsync_WithValidRequest_UpdatesGenre()
    {
        // Arrange
        var existingGenre = new Genre { Id = 1, Name = "ExistingGenre", ParentId = 2 };
        var updateRequest = new GenreUpdateRequest { Id = existingGenre.Id, Name = "UpdatedGenre", ParentId = existingGenre.ParentId };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(existingGenre.Id))
            .ReturnsAsync(existingGenre);
        _mockRepository
            .Setup(repository => repository.UpdateAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _genreService.UpdateGenreAsync(updateRequest);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for UpdateGenreAsync when genre does not exist.
    /// Expects KeyNotFoundException.
    /// </summary>
    [Fact]
    public async Task UpdateGenreAsync_WithNonExistentGenre_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistentGenreId = 999;
        var updateRequest = new GenreUpdateRequest { Id = nonExistentGenreId, Name = "UpdatedGenre" };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(nonExistentGenreId))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _genreService.UpdateGenreAsync(updateRequest));
    }

    /// <summary>
    /// Test for UpdateGenreAsync when genre tries to set itself as parent.
    /// Expects ArgumentException.
    /// </summary>
    [Fact]
    public async Task UpdateGenreAsync_SetItselfAsParent_ThrowsArgumentException()
    {
        // Arrange
        var existingGenre = new Genre { Id = 1, Name = "ExistingGenre" };
        var updateRequest = new GenreUpdateRequest { Id = existingGenre.Id, Name = "UpdatedGenre", ParentId = existingGenre.Id };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(existingGenre.Id))
            .ReturnsAsync(existingGenre);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _genreService.UpdateGenreAsync(updateRequest));
    }

    /// <summary>
    /// Test for RemoveGenreAsync when genre exists.
    /// </summary>
    [Fact]
    public async Task RemoveGenreAsync_WithExistingId_RemovesGenre()
    {
        // Arrange
        var existingGenreId = 1;

        _mockRepository
            .Setup(repository => repository.RemoveAsync(existingGenreId))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _genreService.RemoveGenreAsync(existingGenreId);

        // Assert
        _mockRepository.Verify();
    }

    /// <summary>
    /// Test for InitializePredefinedGenresAsync method.
    /// </summary>
    [Fact]
    public async Task InitializePredefinedGenresAsync_AddsGenres()
    {
        // Arrange
        _mockRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _genreService.InitializePredefinedGenresAsync();

        // Assert
        _mockRepository.Verify(repository => repository.AddAsync(It.IsAny<Genre>()), Times.Exactly(16));
    }

    /// <summary>
    /// Test for GetGenresByGameAliasAsync method.
    /// </summary>
    [Fact]
    public async Task GetGenresByGameAliasAsync_ReturnsGenresOfTheGame()
    {
        // Arrange
        var gameAlias = "TestGame";
        var testGenres = new List<Genre>
        {
            new() { Name = "Test Genre 1" },
            new() { Name = "Test Genre 2" },
        };

        _mockRepository
            .Setup(repository => repository.GetByGameAliasAsync(gameAlias))
            .ReturnsAsync(testGenres);

        // Act
        var genreResponses = await _genreService.GetGenresByGameAliasAsync(gameAlias);

        // Assert
        Assert.NotNull(genreResponses);
        Assert.All(genreResponses, genreResponse => Assert.Contains(genreResponse.Name, testGenres.Select(tg => tg.Name)));
    }

    /// <summary>
    /// Test for GetGenresByParentIdAsync method.
    /// </summary>
    [Fact]
    public async Task GetGenresByParentIdAsync_ReturnsChildGenres()
    {
        // Arrange
        var parentId = 1;
        var testGenres = new List<Genre>
        {
            new() { Name = "Test Genre 1", ParentId = parentId },
            new() { Name = "Test Genre 2", ParentId = parentId },
        };

        _mockRepository
            .Setup(repository => repository.GetByParentIdAsync(parentId))
            .ReturnsAsync(testGenres);

        // Act
        var childGenres = await _genreService.GetGenresByParentIdAsync(parentId);

        // Assert
        Assert.NotNull(childGenres);
        Assert.All(childGenres, childGenre => Assert.Contains(childGenre.Name, testGenres.Select(tg => tg.Name)));
    }
}
