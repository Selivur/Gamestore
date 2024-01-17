using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.GenreDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a genre request.
/// </summary>
public class GenreRequest
{
    /// <summary>
    /// Gets or sets the name of the genre. Required.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional parent ID of the genre.
    /// </summary>
    public int? ParentId { get; set; }
}
