using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a platform entity in the Gamestore database.
/// </summary>
[Index(nameof(Type), IsUnique = true)]
public class Platform
{
    /// <summary>
    /// Gets or sets the unique identifier of the platform.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the platform.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the collection of games associated with the platform.
    /// </summary>
    public ICollection<Game> Games { get; set; }
}
