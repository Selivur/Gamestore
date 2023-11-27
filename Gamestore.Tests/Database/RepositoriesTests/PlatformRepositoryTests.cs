namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Unit tests for the <see cref="PlatformRepository"/> class.
/// </summary>
public class PlatformRepositoryTests
{
    private readonly DbContextOptions<GamestoreContext> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformRepositoryTests"/> class.
    /// </summary>
    public PlatformRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the PlatformRepository
    /// returns a platform when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPlatform()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platform = new Platform { Id = 1, Type = "PC" };
        context.Platforms.Add(platform);
        context.SaveChanges();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platform.Id, result.Id);
        Assert.Equal(platform.Type, result.Type);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the PlatformRepository
    /// returns null when the platform does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

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
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platformName = "PC";
        var platform = new Platform { Type = platformName };
        context.Platforms.Add(platform);
        context.SaveChanges();

        // Act
        var result = await repository.GetByNameAsync(platformName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platformName, result.Type);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the PlatformRepository
    /// returns null when the platform does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platformName = "PC";

        // Act
        var result = await repository.GetByNameAsync(platformName);

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
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platforms = new List<Platform>
            {
                new Platform { Id = 1, Type = "PC" },
                new Platform { Id = 2, Type = "PlayStation" },
                new Platform { Id = 3, Type = "Xbox" },
            };

        context.Platforms.AddRange(platforms);
        context.SaveChanges();

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platforms.Count, result.Count);

        foreach (var platform in platforms)
        {
            Assert.Contains(result, p => p.Type == platform.Type);
        }
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the PlatformRepository
    /// adds a platform to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddPlatformToDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platform = new Platform { Type = "Nintendo Switch" };

        // Act
        await repository.AddAsync(platform);

        // Assert
        var savedPlatform = await context.Platforms.FirstOrDefaultAsync(p => p.Type == "Nintendo Switch");
        Assert.NotNull(savedPlatform);
        Assert.Equal(platform.Type, savedPlatform.Type);
    }

    /// <summary>
    /// Test to verify that the UpdateAsync method of the PlatformRepository
    /// updates a platform in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePlatformInDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platform = new Platform { Id = 1, Type = "PC" };
        context.Platforms.Add(platform);
        context.SaveChanges();

        // Modify the platform
        platform.Type = "Updated PC";

        // Act
        await repository.UpdateAsync(platform);

        // Assert
        var updatedPlatform = await context.Platforms.FirstOrDefaultAsync(p => p.Id == 1);
        Assert.NotNull(updatedPlatform);
        Assert.Equal("Updated PC", updatedPlatform.Type);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the PlatformRepository
    /// removes a platform from the database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemovePlatformFromDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        var platform = new Platform { Id = 1, Type = "PC" };
        context.Platforms.Add(platform);
        context.SaveChanges();

        // Act
        await repository.RemoveAsync("PC");

        // Assert
        var removedPlatform = await context.Platforms.FirstOrDefaultAsync(p => p.Id == 1);
        Assert.Null(removedPlatform);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the PlatformRepository
    /// throws an exception when the platform is not found.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionWhenPlatformNotFound()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.RemoveAsync("nonexistent-platform"));
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the PlatformRepository
    /// throws an exception when SaveChanges fails.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldThrowExceptionWhenSaveChangesFails()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new PlatformRepository(context);

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.AddAsync(new Platform()));
    }
}
