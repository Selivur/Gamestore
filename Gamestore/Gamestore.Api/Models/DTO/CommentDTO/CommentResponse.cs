using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.CommentDTO;

/// <summary>
/// Represents a comment with its associated child comments.
/// </summary>
public class CommentResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the author of the comment.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the body of the comment.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the list of child comments.
    /// This is null if the comment has no child comments.
    /// </summary>
    public List<CommentResponse>? ChildComments { get; set; }

    /// <summary>
    /// Creates a new CommentResponse from a Comment entity.
    /// </summary>
    /// <param name="comment">The Comment entity to convert.</param>
    /// <returns>A CommentResponse that represents the given Comment entity.</returns>
    public static CommentResponse FromComment(Comment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            Name = comment.Name,
            Body = comment.Body,
            ChildComments = comment.ChildComments?.Select(FromComment).ToList(),
        };
    }
}
