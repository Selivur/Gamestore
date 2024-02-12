using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.CommentDTO;

/// <summary>
/// Represents a request to create a new comment.
/// </summary>
public class CommentRequest
{
    /// <summary>
    /// Gets or sets the name of the author of the comment.
    /// This field is required.
    /// </summary>
    [Required(ErrorMessage = "Comment name is required")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the body of the comment.
    /// This field is required.
    /// </summary>
    [Required(ErrorMessage = "Comment body is required")]
    public string Body { get; set; }
}
