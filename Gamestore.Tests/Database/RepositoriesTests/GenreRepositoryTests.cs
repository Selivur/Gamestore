namespace Gamestore.Tests.Database.RepositoriesTests;

public class GenreRepositoryTests
{
    private readonly DbContextOptions<GamestoreContext> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreRepositoryTests"/> class.
    /// </summary>
    public GenreRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GenreRepository
    /// returns a genre when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnGenre()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genre = new Genre { Id = 1, Name = "Adventure" };
        context.Genres.Add(genre);
        context.SaveChanges();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genre.Id, result.Id);
        Assert.Equal(genre.Name, result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByIdAsync method of the GenreRepository
    /// returns null when the genre does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the GenreRepository returns a genre when it exists in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnGenre()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genreName = "Adventure";
        var genre = new Genre { Name = genreName };
        context.Genres.Add(genre);
        context.SaveChanges();

        // Act
        var result = await repository.GetByNameAsync(genreName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreName, result.Name);
    }

    /// <summary>
    /// Test to verify that the GetByNameAsync method of the GenreRepository returns null when the genre does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genreName = "Adventure";

        // Act
        var result = await repository.GetByNameAsync(genreName);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test to verify that the GetAllAsync method of the GenreRepository returns all genres from the database.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGenres()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Adventure" },
            new() { Id = 2, Name = "Action" },
            new() { Id = 3, Name = "RPG" },
        };

        context.Genres.AddRange(genres);
        context.SaveChanges();

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genres.Count, result.Count);

        foreach (var genre in genres)
        {
            Assert.Contains(result, g => g.Name == genre.Name);
        }
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GenreRepository adds a genre to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddGenreToDatabase()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genre = new Genre { Name = "Strategy" };

        // Act
        await repository.AddAsync(genre);

        // Assert
        var savedGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Strategy");
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
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genre = new Genre { Id = 1, Name = "Adventure" };
        context.Genres.Add(genre);
        context.SaveChanges();

        // Modify the genre
        genre.Name = "Updated Adventure";

        // Act
        await repository.UpdateAsync(genre);

        // Assert
        var updatedGenre = await context.Genres.FirstOrDefaultAsync(g => g.Id == 1);
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
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genre = new Genre { Id = 1, Name = "Adventure", ParentId = 1, };
        context.Genres.Add(genre);
        context.SaveChanges();

        // Act
        await repository.RemoveAsync(1);

        // Assert
        var removedGenre = await context.Genres.FirstOrDefaultAsync(g => g.Id == 1);
        Assert.Null(removedGenre);
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GenreRepository throws an exception when the genre is not found.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionWhenGenreNotFound()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repository.RemoveAsync(0));
    }

    /// <summary>
    /// Test to verify that the RemoveAsync method of the GenreRepository throws an exception for a parent genre.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ShouldThrowExceptionForParentGenre()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        var genre = new Genre { Id = 1, Name = "Adventure" };
        context.Genres.Add(genre);
        context.SaveChanges();

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.RemoveAsync(1));
    }

    /// <summary>
    /// Test to verify that the AddAsync method of the GenreRepository throws an exception when SaveChanges fails.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldThrowExceptionWhenSaveChangesFails()
    {
        // Arrange
        using var context = new GamestoreContext(_options);
        var repository = new GenreRepository(context);

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.AddAsync(new Genre()));
    }
}
