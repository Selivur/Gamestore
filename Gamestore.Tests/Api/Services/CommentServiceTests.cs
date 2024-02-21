using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Services;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Repositories.Interfaces;
using Moq;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="CommentService"/> class.
/// </summary>
public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly CommentService _commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentServiceTests"/> class.
    /// </summary>
    public CommentServiceTests()
    {
        _mockCommentRepository = new Mock<ICommentRepository>();
        _commentService = new CommentService(_mockCommentRepository.Object);
        InitializeTestData();
    }

    /// <summary>
    /// Test to verify that the AddCommentAsync method of the CommentService
    /// adds a comment to the repository.
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ShouldAddCommentToRepository()
    {
        // Arrange
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = "Test", Body = "Test body" },
            Action = "Quote",
            ParentId = 1,
        };
        var game = new Game { GameAlias = "TestGame", Name = "Test Game" };

        // Act
        await _commentService.AddCommentAsync(commentWrapper, game);

        // Assert
        _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
    }

    /// <summary>
    /// Test to verify that the GetCommentsByGameIdAsync method of the CommentService
    /// retrieves comments by game id from the repository.
    /// </summary>
    [Fact]
    public async Task GetCommentsByGameIdAsync_ShouldReturnCommentsByGameId()
    {
        // Arrange
        var gameId = 1;

        // Act
        var result = await _commentService.GetCommentsByGameIdAsync(gameId);

        // Assert
        _mockCommentRepository.Verify(r => r.GetAllByGameIdAsync(gameId), Times.Once);
    }

    /// <summary>
    /// Test to verify that the AddCommentAsync method of the CommentService
    /// throws an exception when called with invalid data.
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ShouldThrowException_WhenCalledWithInvalidData()
    {
        // Arrange
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = string.Empty, Body = string.Empty },
            Action = "InvalidAction", // Invalid action
            ParentId = 1,
        };
        var game = new Game { GameAlias = "TestGame", Name = "Test Game" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _commentService.AddCommentAsync(commentWrapper, game));
    }

    /// <summary>
    /// Test to verify that the RemoveCommentAsync method of the CommentService
    /// throws an exception when called with a non-existing comment ID.
    /// </summary>
    [Fact]
    public async Task RemoveCommentAsync_ShouldThrowException_WhenCalledWithNonExistingCommentId()
    {
        // Arrange
        var nonExistingCommentId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _commentService.RemoveCommentAsync(nonExistingCommentId));
    }

    /// <summary>
    /// Test to verify that the RemoveCommentAsync method of the CommentService
    /// updates the body of child comments when they have a status of Quote.
    /// </summary>
    [Fact]
    public async Task RemoveCommentAsync_ShouldUpdateChildComments_WhenTheyHaveQuoteStatus()
    {
        // Arrange
        var commentId = 2;

        // Act
        await _commentService.RemoveCommentAsync(commentId);

        // Assert
        _mockCommentRepository.Verify(r => r.UpdateAsync(It.Is<Comment>(c => c.Body == "A comment/quote was deleted" && (c.ChildComments == null || c.ChildComments.All(cc => cc.Body == "A comment/quote was deleted")))), Times.Once);
    }

    /// <summary>
    /// Test to verify that the AddCommentAsync method of the CommentService
    /// correctly formats the body of the comment when the action is "Quote".
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ShouldFormatBodyCorrectly_WhenActionIsQuote()
    {
        // Arrange
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = "Test", Body = "Test body" },
            Action = "Quote",
            ParentId = 1,
        };
        var game = new Game { GameAlias = "TestGame", Name = "Test Game" };

        // Act
        await _commentService.AddCommentAsync(commentWrapper, game);

        // Assert
        _mockCommentRepository.Verify(
            r => r.AddAsync(It.Is<Comment>(
                c => c.Body.Contains("<i>") && c.Body.Contains("</i><br/>"))),
            Times.Once);
    }

    /// <summary>
    /// Test to verify that the AddCommentAsync method of the CommentService
    /// correctly formats the body of the comment when the action is "Reply".
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ShouldFormatBodyCorrectly_WhenActionIsReply()
    {
        // Arrange
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = "Test", Body = "Test body" },
            Action = "Reply",
            ParentId = 1,
        };
        var game = new Game { GameAlias = "TestGame", Name = "Test Game" };

        // Act
        await _commentService.AddCommentAsync(commentWrapper, game);

        // Assert
        _mockCommentRepository.Verify(
            r => r.AddAsync(It.Is<Comment>(
                c => c.Body.Contains("[<a href='/game/GameAlias1#comment") && c.Body.Contains("</a>]"))),
            Times.Once);
    }

    /// <summary>
    /// Test to verify that the AddCommentAsync method of the CommentService
    /// correctly formats the body of the comment when parentId is null.
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ShouldNotChangeBody_WhenParentIdIsNull()
    {
        // Arrange
        var commentWrapper = new CommentWrapper
        {
            Comment = new CommentRequest { Name = "Test", Body = "Test body" },
            Action = "Quote",
            ParentId = null, // No parent comment
        };
        var game = new Game { GameAlias = "TestGame", Name = "Test Game" };

        // Act
        await _commentService.AddCommentAsync(commentWrapper, game);

        // Assert
        _mockCommentRepository.Verify(
            r => r.AddAsync(It.Is<Comment>(
                c => c.Body == commentWrapper.Comment.Body)),
            Times.Once);
    }

    /// <summary>
    /// Test to verify that the RemoveCommentAsync method of the CommentService
    /// does not update the body of child comments when they are null.
    /// </summary>
    [Fact]
    public async Task RemoveCommentAsync_ShouldNotUpdateChildComments_WhenTheyAreNull()
    {
        // Arrange
        var commentId = 3;

        // Act
        await _commentService.RemoveCommentAsync(commentId);

        // Assert
        _mockCommentRepository.Verify(
            r => r.UpdateAsync(It.Is<Comment>(
                c => c.Body == "A comment/quote was deleted" &&
                     c.ChildComments == null)),
            Times.Once);
    }

    /// <summary>
    /// Test to verify that the RemoveCommentAsync method of the CommentService
    /// updates the body of child comments when they are not null.
    /// </summary>
    [Fact]
    public async Task RemoveCommentAsync_ShouldUpdateChildComments_WhenTheyAreNotNull()
    {
        // Arrange
        var commentId = 2; // This comment has child comments
        var commentWithChildren = new Comment
        {
            Id = commentId,
            Body = "Original body",
            ChildComments = new List<Comment>
            {
                new() { Id = 3, Body = "Child comment body", Status = CommentStatus.Quote },
            },
        };
        _mockCommentRepository.Setup(r => r.GetByIdWithChildrenAsync(commentId)).ReturnsAsync(commentWithChildren);

        // Act
        await _commentService.RemoveCommentAsync(commentId);

        // Assert
        _mockCommentRepository.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Once);
        _mockCommentRepository.Verify(r => r.UpdateAsync(It.Is<Comment>(c => c.Body == "A comment/quote was deleted" && c.ChildComments != null && c.ChildComments.All(cc => cc.Body == "A comment/quote was deleted"))), Times.Once);
    }

    /// <summary>
    /// Initializes the test data.
    /// </summary>
    private void InitializeTestData()
    {
        var testGames = new List<Game>
        {
            new() { Id = 1, GameAlias = "game1", Name = "Game 1" },
            new() { Id = 2, GameAlias = "game2", Name = "Game 2" },
        };

        var testComments = new List<Comment>
        {
            new() { Id = 1, Name = "Test Comment 1", Body = "This is a test comment 1.", Game = testGames[0] },
            new() { Id = 2, Name = "Test Comment 2", Body = "This is a test comment 2.", ParentId = 1, Game = testGames[0], Status = CommentStatus.Quote },
            new() { Id = 3, Name = "Test Comment 3", Body = "This is a test comment 3.", Game = testGames[1] },
        };

        _mockCommentRepository.Setup(r => r.GetAllByGameIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => testComments.Where(c => c.Game.Id == id));
        _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => testComments.FirstOrDefault(c => c.Id == id));
        _mockCommentRepository.Setup(r => r.GetByIdWithChildrenAsync(It.IsAny<int>())).ReturnsAsync((int id) => testComments.FirstOrDefault(c => c.Id == id));
    }
}
