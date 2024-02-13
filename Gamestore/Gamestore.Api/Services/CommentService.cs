using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
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
        CommentStatus? status = DetermineCommentStatus(commentWrapper.Action);

        Comment comment = new()
        {
            Name = commentWrapper.Comment.Name,
            Body = commentWrapper.Comment.Body,
            Game = game,
            ParentId = commentWrapper.ParentId,
            Status = status,
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
    public async Task RemoveCommentAsync(int id)
    {
        // TODO add check comment owner
        var comment = await _commentRepository.GetByIdWithChildrenAsync(id);

        comment.Body = "A comment/quote was deleted";

        if (comment.ChildComments != null)
        {
            foreach (var childComment in comment.ChildComments.Where(c => c.Status == CommentStatus.Quote))
            {
                childComment.Body = "A comment/quote was deleted";
            }
        }

        await _commentRepository.UpdateAsync(comment);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommentResponse>> GetCommentsByGameIdAsync(int gameId)
    {
        var comments = await _commentRepository.GetAllByGameIdAsync(gameId);

        var commentResponses = comments.Where(c => c.ParentId == null).Select(CommentResponse.FromComment).ToList();

        return commentResponses;
    }

    private static CommentStatus? DetermineCommentStatus(string action)
    {
        if (action == "Quote")
        {
            return CommentStatus.Quote;
        }
        else if (action == "Reply")
        {
            return CommentStatus.Reply;
        }

        return null;
    }
}
