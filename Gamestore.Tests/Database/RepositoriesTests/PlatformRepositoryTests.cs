using Gamestore.Database.Services.Interfaces;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Unit tests for the <see cref="PlatformRepository"/> class.
/// </summary>
public class PlatformRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly PlatformRepository _repository;
    private readonly Mock<IDataBaseLogger> _mockLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformRepositoryTests"/> class.
    /// </summary>
    public PlatformRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockLogger = new Mock<IDataBaseLogger>();
        _context = new GamestoreContext(_options);
        _repository = new PlatformRepository(_context, _mockLogger.Object);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the PlatformRepository
    /// returns a platform when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPlatform()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("PC", result.Type);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the PlatformRepository
    /// returns null when the platform does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999); // non-existent platform Id

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the PlatformRepository
    /// returns a platform when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnPlatform()
    {
        // Act
        var result = await _repository.GetByNameAsync("PC");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PC", result.Type);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the PlatformRepository
    /// returns null when the platform does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByNameAsync("NonExistentPlatform");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetAllAsync method of the PlatformRepository
    /// returns all platforms from the database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPlatforms()
    {
        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Type == "PC");
        Assert.Contains(result, p => p.Type == "PlayStation");
        Assert.Contains(result, p => p.Type == "Xbox");
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the PlatformRepository
    /// adds a platform to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddPlatformToDatabase()
    {
        // Arrange
        await _repository.AddAsync(new Platform { Id = 4, Type = "Nintendo Switch" });
        await _context.SaveChangesAsync();

        // Act
        var savedPlatform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == 4);

        // Assert
        Assert.NotNull(savedPlatform);
        Assert.Equal("Nintendo Switch", savedPlatform.Type);
    }

    /// <summary>
    /// Test to verify that the UpdateAsync method of the PlatformRepository
    /// updates a platform in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePlatformInDatabase()
    {
        // Arrange
        var platform = await _context.Platforms.FirstAsync(p => p.Id == 1);
        platform.Type = "Updated PC";

        // Act
        await _repository.UpdateAsync(platform);
        await _context.SaveChangesAsync();

        // Assert
        var updatedPlatform = await _context.Platforms.FirstAsync(p => p.Id == 1);
        Assert.Equal(platform.Type, updatedPlatform.Type);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the PlatformRepository
    /// removes a platform from the database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemovePlatformFromDatabase()
    {
        // Arrange
        var platform = await _context.Platforms.FirstAsync(p => p.Id == 1);

        // Act
        await _repository.RemoveAsync(platform.Id);
        await _context.SaveChangesAsync();

        // Assert
        var removedPlatform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == platform.Id);
        Assert.Null(removedPlatform);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the PlatformRepository
    /// throws an exception when the platform is not found.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionWhenPlatformNotFound()
    {
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.RemoveAsync(999)); // non-existent platform Id
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the PlatformRepository
    /// throws an exception when SaveChanges fails.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldThrowExceptionWhenSaveChangesFails()
    {
        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _repository.AddAsync(new Platform()));
    }

    /// <summary>
    /// Test to verify that the GetByGameAliasAsync method of the PlatformRepository
    /// returns a list of platforms when they exist in the database for the provided game alias.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldReturnPlatformList()
    {
        // Arrange
        var game = new Game { Name = "Test Game", GameAlias = "Alias1", Platforms = new List<Platform>() };
        game.Platforms.Add(await _context.Platforms.FirstAsync(p => p.Id == 1));
        game.Platforms.Add(await _context.Platforms.FirstAsync(p => p.Id == 2));
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByGameAliasAsync("Alias1")).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Type == "PC");
        Assert.Contains(result, p => p.Type == "PlayStation");
    }

    /// <summary>
    /// Test to verify that the GetByGameAliasAsync method of the PlatformRepository
    /// return an empty list when no platforms are associated with the provided game alias.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldReturnEmptyList()
    {
        // Arrange
        _context.Games.Add(new Game { Name = "Test Game", GameAlias = "Alias2" });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByGameAliasAsync("Alias2")).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Test to verify that the GetByGameAliasAsync method of the PlatformRepository
    /// throws an exception when the game alias does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldThrowExceptionWhenGameAliasNotFound()
    {
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.GetByGameAliasAsync("NonExistentAlias"));
    }

    /// <summary>
    /// Cleans up the resources after each test by deleting the database.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    private void InitializeTestData()
    {
        var platforms = new List<Platform>
        {
            new() { Id = 1, Type = "PC" },
            new() { Id = 2, Type = "PlayStation" },
            new() { Id = 3, Type = "Xbox" },
        };

        _context.Platforms.AddRange(platforms);
        _context.SaveChanges();
    }
}
