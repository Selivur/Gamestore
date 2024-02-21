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

        string body = await FormatCommentBodyAsync(commentWrapper.ParentId, commentWrapper.Comment.Body, status);

        Comment comment = new()
        {
            Name = commentWrapper.Comment.Name,
            Body = body,
            Game = game,
            ParentId = commentWrapper.ParentId,
            Status = status,
        };

        await _commentRepository.AddAsync(comment);
    }

    /// <inheritdoc/>
    public async Task RemoveCommentAsync(int id)
    {
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

    private async Task<string> FormatCommentBodyAsync(int? parentId, string body, CommentStatus? status)
    {
        if (parentId == null)
        {
            return body;
        }

        var parentComment = await _commentRepository.GetByIdAsync(parentId.Value);

        return status switch
        {
            CommentStatus.Quote => $"<i>{parentComment.Body}</i><br/>{body}",
            CommentStatus.Reply => $"[<a href='/game/GameAlias1#comment{parentComment.Id}'>{parentComment.Name}</a>], {body}",
            _ => throw new ArgumentException($"Invalid comment status: {status}"),
        };
    }

    private static CommentStatus? DetermineCommentStatus(string action)
    {
        return action switch
        {
            "Quote" => CommentStatus.Quote,
            "Reply" => CommentStatus.Reply,
            _ => null,
        };
    }
}
