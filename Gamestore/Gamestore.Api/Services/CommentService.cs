using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentService"/> class.
    /// </summary>
    /// <param name="commentRepository">The comment repository providing data access for the service.</param>
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    /// <inheritdoc/>
    public async Task AddCommentAsync(CommentWrapper commentWrapper, Game game)
    {
        Comment comment = new()
        {
            Name = commentWrapper.Comment.Name,
            Body = commentWrapper.Comment.Body,
            Game = game,
            ParentId = commentWrapper.ParentId,
        };

        await _commentRepository.AddAsync(comment);
    }

    /// <inheritdoc/>
    public Task<CommentResponse?> GetCommentByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task UpdateCommentAsync(CommentRequest commentRequest)
    {
        // TODO add check ref comment gameId != gameId
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task RemoveCommentAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommentResponse>> GetCommentsByGameIdAsync(int gameId)
    {
        var comments = await _commentRepository.GetAllByGameIdAsync(gameId);

        var commentResponses = comments.Where(c => c.ParentId == null).Select(CommentResponse.FromComment).ToList();

        return commentResponses;
    }
}
