namespace Gamestore.Database.Entities.Enums;

/// <summary>
/// Specifies the status of a comment.
/// </summary>
public enum CommentStatus
{
    /// <summary>
    /// The comment is a reply to another comment.
    /// </summary>
    Reply,

    /// <summary>
    /// The comment is a quote from another comment or source.
    /// </summary>
    Quote,
}