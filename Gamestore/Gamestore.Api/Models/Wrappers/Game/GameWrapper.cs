using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GameDTO;

namespace Gamestore.Api.Models.Wrappers.Game;

/// <summary>
/// Represents a wrapper for a game, including related information like genres, platforms, and publisher.
/// </summary>
public class GameWrapper
{
    /// <summary>
    /// Gets or sets the game information.
    /// </summary>
    [JsonPropertyName("game")]
    public GameRequest GameRequest { get; set; }

    /// <summary>
    /// Gets or sets the array of genre IDs associated with the game.
    /// </summary>
    [JsonPropertyName("genres")]
    public int[] GenresId { get; set; }

    /// <summary>
    /// Gets or sets the array of platform IDs associated with the game.
    /// </summary>
    [JsonPropertyName("platforms")]
    public int[] PlatformsId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the publisher associated with the game.
    /// </summary>
    [JsonPropertyName("publisher")]
    public int PublisherId { get; set; }
}