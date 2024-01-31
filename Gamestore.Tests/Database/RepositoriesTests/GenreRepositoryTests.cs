namespace Gamestore.Tests.Database.RepositoriesTests;

public class GenreRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly GenreRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreRepositoryTests"/> class.
    /// </summary>
    public GenreRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GamestoreContext(_options);
        _repository = new GenreRepository(_context);

        SeedDataAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Cleans up the resources after each test by deleting the database.
    /// </summary>
    public void Dispose()
    {
        using var context = new GamestoreContext(_options);
        context.Database.EnsureDeleted();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GenreRepository
    /// returns a genre when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnGenre()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Adventure", result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GenreRepository
    /// returns null when the genre does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the GenreRepository returns a genre when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnGenre()
    {
        // Act
        var result = await _repository.GetByNameAsync("Action");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Action", result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the GenreRepository returns null when the genre does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull()
    {
        // Arrange
        var genreName = "Non-Existent";

        // Act
        var result = await _repository.GetByNameAsync(genreName);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetAllAsync method of the GenreRepository returns all genres from the database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGenres()
    {
        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GenreRepository adds a genre to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddGenreToDatabase()
    {
        // Arrange
        var genre = new Genre { Name = "Strategy" };

        // Act
        await _repository.AddAsync(genre);

        // Assert
        var savedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == "Strategy");
        Assert.NotNull(savedGenre);
        Assert.Equal(genre.Name, savedGenre.Name);
    }

    /// <summary>
    /// Test to verify that the UpdateAsync method of the GenreRepository updates a genre in the database.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateGenreInDatabase()
    {
        // Arrange
        var genre = await _context.Genres.SingleAsync(g => g.Id == 1);
        genre.Name = "Updated Adventure";

        // Act
        await _repository.UpdateAsync(genre);

        // Assert
        var updatedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == 1);
        Assert.NotNull(updatedGenre);
        Assert.Equal("Updated Adventure", updatedGenre.Name);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GenreRepository removes a genre from the database.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldRemoveGenreFromDatabase()
    {
        // Arrange
        var genre = new Genre { Id = 4, Name = "Horror", ParentId = 1 };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        // Act
        await _repository.RemoveAsync(4);

        // Assert
        var removedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == 4);
        Assert.Null(removedGenre);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GenreRepository throws an exception when the genre is not found.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionWhenGenreNotFound()
    {
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.RemoveAsync(0));
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GenreRepository throws an exception for a parent genre.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionForParentGenre()
    {
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.RemoveAsync(1));
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GenreRepository throws an exception when SaveChanges fails.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldThrowExceptionWhenSaveChangesFails()
    {
        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _repository.AddAsync(new Genre()));
    }

    /// <summary>
    /// Test to verify that the GetByGameAliasAsync method of the GenreRepository
    /// returns genres for the game with given alias when the game exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldReturnGenresForGame()
    {
        // Arrange
        var gameAlias = "TestGame";
        var game = new Game
        {
            GameAlias = gameAlias,
            Name = "Test Game",
            Genre = new List<Genre>()
            {
                await _context.Genres.FirstAsync(g => g.Id == 1),
                await _context.Genres.FirstAsync(g => g.Id == 2),
            },
        };
        _context.Games.Add(game);
        _context.SaveChanges();

        // Act
        var result = (await _repository.GetByGameAliasAsync(gameAlias)).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(game.Genre.Count, result.Count);
        foreach (var genre in game.Genre)
        {
            Assert.Contains(result, g => g.Name == genre.Name);
        }
    }

    /// <summary>
    /// Test to verify that the GetByGameAliasAsync method of the GenreRepository
    /// throws an exception when the game does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByGameAliasAsync_ShouldThrowExceptionWhenGameNotFound()
    {
        // Arrange
        var gameAlias = "NonExistentGame";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByGameAliasAsync(gameAlias));
        Assert.Contains(gameAlias, ex.Message);
    }

    /// <summary>
    /// Test to verify that the GetByParentIdAsync method of the GenreRepository
    /// returns genres by the parent id when genres exist with the parent id in the database.
    /// </summary>
    [Fact]
    public async Task GetByParentIdAsync_ShouldReturnGenresByParentId()
    {
        // Arrange
        var parentGenreId = 9;
        var genres = new List<Genre>
        {
            new() { Id = 28, Name = "Fantasy", ParentId = parentGenreId },
            new() { Id = 29, Name = "Sci-Fi", ParentId = parentGenreId },
        };
        _context.Genres.AddRange(genres);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByParentIdAsync(parentGenreId)).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(genres.Count, result.Count);
        foreach (var genre in genres)
        {
            Assert.Contains(result, g => g.Name == genre.Name);
        }
    }

    /// <summary>
    /// Test to verify that the GetByParentIdAsync method of the GenreRepository
    /// returns an empty list when no genre exists with the parent id in the database.
    /// </summary>
    [Fact]
    public async Task GetByParentIdAsync_ShouldReturnEmptyListWhenNotFound()
    {
        // Arrange
        var nonExistentParentId = 9999;

        // Act
        var result = (await _repository.GetByParentIdAsync(nonExistentParentId)).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Seeds the database with initial data for testing.
    /// </summary>
    private async Task SeedDataAsync()
    {
        using var context = new GamestoreContext(_options);
        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Adventure" },
            new() { Id = 2, Name = "Action" },
            new() { Id = 3, Name = "RPG" },
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }
}
