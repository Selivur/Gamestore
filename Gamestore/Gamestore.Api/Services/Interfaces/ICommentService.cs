using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Database.Entities;

namespace Gamestore.Api.Services.Interfaces;

public interface ICommentService
{
    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="commentRequest">The comment to add.</param>
    /// <param name="game">The game to which the comment will be added.</param>
    Task AddCommentAsync(CommentRequest commentRequest, Game game);

    /// <summary>
    /// Retrieves a comment by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve.</param>
    /// <returns>The retrieved comment.</returns>
    Task<CommentResponse?> GetCommentByIdAsync(int id);

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="commentRequest">The comment to update.</param>
    Task UpdateCommentAsync(CommentRequest commentRequest);

    /// <summary>
    /// Removes a comment by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to remove.</param>
    Task RemoveCommentAsync(int id);

    /// <summary>
    /// Retrieves all comments associated with a specific game.
    /// </summary>
    /// <param name="id">The ID of game.</param>
    /// <returns>A collection of comments associated with the game.</returns>
    Task<IEnumerable<CommentResponse>> GetCommentsByGameIdAsync(int id);
}