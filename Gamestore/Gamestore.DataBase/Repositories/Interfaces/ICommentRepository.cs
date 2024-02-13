using Gamestore.Database.Entities;

namespace Gamestore.Database.Repositories.Interfaces;

/// <summary>
/// Defines the operations for managing comments in the database.
/// </summary>
public interface ICommentRepository
{
    /// <summary>
    /// Creates a new comment in the database.
    /// </summary>
    /// <param name="comment">The comment to create.</param>
    Task AddAsync(Comment comment);

    /// <summary>
    /// Retrieves a comment from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve.</param>
    /// <returns>The retrieved comment.</returns>
    Task<Comment?> GetByIdAsync(int id);

    /// <summary>
    /// Updates an existing comment in the database.
    /// </summary>
    /// <param name="comment">The comment to update.</param>
    /// <returns>The updated comment.</returns>
    Task UpdateAsync(Comment comment);

    /// <summary>
    /// Deletes a comment from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    Task RemoveAsync(int id);

    /// <summary>
    /// Retrieves all comments associated with a specific game from the database.
    /// </summary>
    /// <param name="gameId">The ID of the comments to retrieve.</param>
    /// <returns>A collection of comments associated with the game.</returns>
    Task<IEnumerable<Comment>> GetAllByGameIdAsync(int gameId);

    /// <summary>
    /// Gets a comment by its ID, including its child comments.
    /// </summary>
    /// <param name="id">The ID of the comment.</param>
    /// <returns>The comment with the specified ID, or null if no such comment exists.</returns>
    Task<Comment?> GetByIdWithChildrenAsync(int id);
}