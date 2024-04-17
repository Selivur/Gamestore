using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Services.Interfaces;
using Moq;

namespace Gamestore.Tests.Database.RepositoriesTests;

/// <summary>
/// Contains unit tests for the <see cref="CommentRepository"/> class.
/// </summary>
public class CommentRepositoryTests : IDisposable
{
    private readonly DbContextOptions<GamestoreContext> _options;
    private readonly GamestoreContext _context;
    private readonly CommentRepository _repository;
    private readonly Mock<IDataBaseLogger> _mockLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepositoryTests"/> class.
    /// </summary>
    public CommentRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<GamestoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockLogger = new Mock<IDataBaseLogger>();
        _context = new GamestoreContext(_options);
        _repository = new CommentRepository(_context, _mockLogger.Object);

        InitializeTestData();
    }

    /// <summary>
    /// Tests the GetByIdAsync method to ensure it returns the correct comment.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectComment()
    {
        // Arrange
        var commentId = 1;

        // Act
        var result = await _repository.GetByIdAsync(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
    }

    /// <summary>
    /// Tests the GetAllByGameIdAsync method to ensure it returns the correct comments.
    /// </summary>
    [Fact]
    public async Task GetAllByGameIdAsync_ReturnsCorrectComments()
    {
        // Arrange
        var gameId = 1;

        // Act
        var result = await _repository.GetAllByGameIdAsync(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, comment => Assert.Equal(gameId, comment.Game.Id));
    }

    /// <summary>
    /// Tests the AddAsync method to ensure it adds a comment correctly.
    /// </summary>
    [Fact]
    public async Task AddAsync_AddsCommentCorrectly()
    {
        // Arrange
        var newComment = new Comment { Id = 4, Name = "Test Comment 4", Body = "This is a test comment 4.", Game = _context.Games.First() };

        // Act
        await _repository.AddAsync(newComment);

        // Assert
        Assert.Contains(newComment, _context.Comments);
    }

    /// <summary>
    /// Tests the RemoveAsync method to ensure it removes a comment correctly.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_RemovesCommentCorrectly()
    {
        // Arrange
        var commentId = 1;

        // Act
        await _repository.RemoveAsync(commentId);

        // Assert
        Assert.DoesNotContain(_context.Comments, comment => comment.Id == commentId);
    }

    /// <summary>
    /// Tests the RemoveAsync method to ensure it throws an exception when trying to remove a comment that does not exist.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ThrowsExceptionWhenCommentDoesNotExist()
    {
        // Arrange
        var nonExistentCommentId = 999;

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveAsync(nonExistentCommentId));
        Assert.Equal($"No comment found with the ID '{nonExistentCommentId}'. (Parameter 'id')", exception.Message);
    }

    /// <summary>
    /// Tests the UpdateAsync method to ensure it updates a comment correctly.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdatesCommentCorrectly()
    {
        // Arrange
        var updatedComment = _context.Comments.First();
        updatedComment.Name = "Updated Comment";

        // Act
        await _repository.UpdateAsync(updatedComment);

        // Assert
        Assert.Equal("Updated Comment", _context.Comments.First().Name);
    }

    /// <summary>
    /// Tests the GetByIdWithChildrenAsync method to ensure it returns the correct comment with its child comments.
    /// </summary>
    [Fact]
    public async Task GetByIdWithChildrenAsync_ReturnsCorrectCommentWithChildren()
    {
        // Arrange
        var commentId = 1;

        // Act
        var result = await _repository.GetByIdWithChildrenAsync(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.NotNull(result.ChildComments);
        Assert.All(result.ChildComments, childComment => Assert.Equal(commentId, childComment.ParentId));
    }

    /// <summary>
    /// Disposes the resources used by the test class.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initializes the test data.
    /// </summary>
    private void InitializeTestData()
    {
        var testGames = new List<Game>
        {
            new() { Id = 1, GameAlias = "game1", Name = "Game 1", Description = "A test game" },
            new() { Id = 2, GameAlias = "game2", Name = "Game 2", Description = "A test game" },
        };

        var testComments = new List<Comment>
        {
            new() { Id = 1, Name = "Test Comment 1", Body = "This is a test comment 1.", Game = testGames[0] },
            new() { Id = 2, Name = "Test Comment 2", Body = "This is a test comment 2.", ParentId = 1, Game = testGames[0], Status = CommentStatus.Quote },
            new() { Id = 3, Name = "Test Comment 3", Body = "This is a test comment 3.", Game = testGames[1] },
        };

        testComments[1].ParentComment = testComments[0];

        testComments[0].ChildComments = new List<Comment> { testComments[1] };

        _context.Games.AddRange(testGames);
        _context.Comments.AddRange(testComments);

        _context.SaveChanges();
    }
}
