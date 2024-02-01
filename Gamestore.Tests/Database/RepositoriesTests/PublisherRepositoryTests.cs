namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Represents tests for the <see cref="PublisherRepository"/>.
/// </summary>
public class PublisherRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly PublisherRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherRepositoryTests"/> class.
    /// </summary>
    public PublisherRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GamestoreContext(_options);
        _repository = new PublisherRepository(_context);

        InitializeTestData();
    }

    /// <summary>
    /// Test to verify if GetByIdAsync method fetches a publisher from database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPublisher()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Valve", result.CompanyName);
    }

    /// <summary>
    /// Test to verify if GetByNameAsync method fetches a publisher from database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnPublisher()
    {
        // Act
        var result = await _repository.GetByNameAsync("Bethesda");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Bethesda", result.CompanyName);
    }

    /// <summary>
    /// Test to verify if GetAllAsync method fetches all publishers from database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPublishers()
    {
        var result = (await _repository.GetAllAsync()).ToList();
        Assert.Equal(4, result.Count);
    }

    /// <summary>
    /// Test to verify if AddAsync method adds a new publisher to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddNewPublisherToDatabase()
    {
        // Arrange
        var newPublisher = new Publisher
        {
            Id = 40,
            CompanyName = "New Publisher",
            Description = "Publisher Description",
            HomePage = "https://publisherwebsite.com",
        };

        // Act
        await _repository.AddAsync(newPublisher);

        var result = await _context.Publishers.FindAsync(40);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(40, result.Id);
        Assert.Equal("New Publisher", result.CompanyName);
        Assert.Equal("Publisher Description", result.Description);
        Assert.Equal("https://publisherwebsite.com", result.HomePage);
    }

    /// <summary>
    /// Test to verify if UpdateAsync method updates a publisher in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePublisherInDatabase()
    {
        // Arrange
        var publisher = await _context.Publishers.FindAsync(2);
        publisher.CompanyName = "Updated Sega";
        await _repository.UpdateAsync(publisher);

        // Act
        var updatedPublisher = await _context.Publishers.FindAsync(2);

        // Assert
        Assert.NotNull(updatedPublisher);
        Assert.Equal("Updated Sega", updatedPublisher.CompanyName);
    }

    /// <summary>
    /// Test to verify if RemoveAsync method removes a publisher from database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemovePublisherFromDatabase()
    {
        // Act
        await _repository.RemoveAsync(3);

        // Assert
        var result = await _context.Publishers.FindAsync(3);
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify if GetByGameAliasAsync method fetches a publisher from the database for a specified game.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldReturnPublishersForGame()
    {
        var publisher1 = new Publisher
        {
            Id = 50,
            CompanyName = "Company1",
            Description = "Publisher Description",
            HomePage = "https://publisherwebsite.com",
        };
        var game = new Game
        {
            GameAlias = "TestGame",
            Name = "Test Game",
            Publisher = publisher1,
        };

        _context.Publishers.Add(publisher1);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var fetchedPublisher = await _repository.GetByGameAliasAsync("TestGame");

        // Assert
        Assert.NotNull(fetchedPublisher);
        Assert.Contains(fetchedPublisher, p => p.CompanyName == "Company1");
    }

    /// <summary>
    /// Test to verify if GetByGameAliasAsync method throws an ArgumentException when a nonexistent game alias is provided.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldThrowArgumentExceptionForNonExistentGame()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _repository.GetByGameAliasAsync("NonExistentGameAlias"));
    }

    /// <summary>
    /// Cleanup method to be called after each tests.
    /// </summary>
    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }

    private void InitializeTestData()
    {
        // Create publisher entities and add to the db context.
        var publishers = new List<Publisher>
        {
            new()
            {
                Id = 1,
                CompanyName = "Valve",
                Description = "Valve Description",
                HomePage = "https://www.valvesoftware.com/",
            },
            new()
            {
                Id = 2,
                CompanyName = "Bethesda",
                Description = "Bethesda Description",
                HomePage = "https://bethesda.net/",
            },
            new()
            {
                Id = 3,
                CompanyName = "Sega",
                Description = "Sega Description",
                HomePage = "https://www.sega.com/",
            },
            new()
            {
                Id = 4,
                CompanyName = "Activision",
                Description = "Activision Description",
                HomePage = "https://www.activision.com/",
            },
        };
        _context.Publishers.AddRange(publishers);

        _context.SaveChanges();
    }

    private void Cleanup()
    {
        foreach (var entity in _context.Publishers)
        {
            _context.Remove(entity);
        }

        _context.SaveChanges();
    }
}
