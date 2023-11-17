using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.GameDTO;

/// <summary>
/// Represents a request for creating or updating a game.
/// </summary>
public class GameRequest
{
    /// <summary>
    /// Gets or sets the alias of the game.
    /// </summary>
    public string? GameAlias { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the game.
    /// </summary>
    public string? Description { get; set; }
}