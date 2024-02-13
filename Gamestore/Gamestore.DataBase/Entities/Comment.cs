using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gamestore.Database.Entities.Enums;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a comment in the database.
/// </summary>
public class Comment
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name associated with the comment.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the body of the comment.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the status of the comment.
    /// This is null if the status is not specified.
    /// </summary>
    public CommentStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the optional parent comment identifier.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent comment of this comment.
    /// </summary>
    [ForeignKey("ParentId")]
    public Comment ParentComment { get; set; }

    /// <summary>
    /// Gets or sets the сhild comments of this comment.
    /// </summary>
    public ICollection<Comment>? ChildComments { get; set; }

    /// <summary>
    /// Gets or sets the game associated with the comment.
    /// </summary>
    public Game Game { get; set; }
}
