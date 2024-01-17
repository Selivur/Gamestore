using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a genre entity in the Gamestore database.
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class Genre
{
    /// <summary>
    /// Gets or sets the unique identifier of the genre.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional parent genre identifier.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent genre associated with the current genre.
    /// </summary>
    [ForeignKey("ParentId")]
    public Genre ParentGenre { get; set; }

    /// <summary>
    /// Gets or sets the collection of games associated with the genre.
    /// </summary>
    public ICollection<Game> Games { get; set; }
}
