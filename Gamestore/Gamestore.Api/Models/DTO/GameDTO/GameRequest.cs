using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gamestore.Api.Models.DTO.GameDTO;

/// <summary>
/// Represents a request for creating or updating a game.
/// </summary>
public class GameRequest
{
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the alias of the game.
    /// </summary>
    [JsonPropertyName("key")]
    public string? GameAlias { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the game.
    /// </summary>
    public string Description { get; set; }

    [JsonPropertyName("discontinued")]
    public int Discount { get; set; }

    public int Price { get; set; }

    public int UnitInStock { get; set; }
}