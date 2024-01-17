using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a publisher entity in the Gamestore database.
/// </summary>
[Index(nameof(CompanyName), IsUnique = true)]
public class Publisher
{
    /// <summary>
    /// Gets or sets the unique identifier of the publisher.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the company name of the publisher.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the description of the publisher.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the home page URL of the publisher.
    /// </summary>
    public string HomePage { get; set; }

    /// <summary>
    /// Gets or sets the collection of games associated with the publisher.
    /// </summary>
    public ICollection<Game> Games { get; set; }
}
