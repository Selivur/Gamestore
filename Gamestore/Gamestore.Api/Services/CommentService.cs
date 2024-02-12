using Gamestore.Api.Models.DTO.CommentDTO;
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
    public async Task AddCommentAsync(CommentRequest commentRequest, Game game)
    {
        // TODO add check ref comment id != id
        // TODO add reply(external method)
        Comment comment = new()
        {
            Name = commentRequest.Name,
            Body = commentRequest.Body,
            Game = game,
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
        // TODO add check ref comment id != id
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task RemoveCommentAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommentResponse>> GetCommentsByGameIdAsync(int id)
    {
        var comments = await _commentRepository.GetAllByGameIdAsync(id);

        var commentResponses = comments.Select(CommentResponse.FromComment).ToList();

        return commentResponses;
    }
}
