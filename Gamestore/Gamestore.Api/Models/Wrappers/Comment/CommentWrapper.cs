using Gamestore.Api.Models.DTO.CommentDTO;

namespace Gamestore.Api.Models.Wrappers.Comment;

/// <summary>
/// Represents a comment with its associated parent comment ID.
/// </summary>
public class CommentWrapper
{
    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public CommentRequest Comment { get; set; }

    /// <summary>
    /// Gets or sets the ID of the parent comment.
    /// This is null if the comment is not a reply to another comment.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the action to be performed on the comment.
    /// This could be "reply", "quote", etc.
    /// </summary>
    public string Action { get; set; }
}
