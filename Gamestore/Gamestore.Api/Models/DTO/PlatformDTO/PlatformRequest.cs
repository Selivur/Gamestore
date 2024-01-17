using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.PlatformDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a platform request.
/// </summary>
public class PlatformRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the platform.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the platform. Required.
    /// </summary>
    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }
}
