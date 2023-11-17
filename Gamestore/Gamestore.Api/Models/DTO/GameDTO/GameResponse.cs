namespace Gamestore.Api.Models.DTO.GameDTO;

/// <summary>
/// Represents a response containing game information.
/// </summary>
public class GameResponse
{
    /// <summary>
    /// Gets or sets the alias of the game.
    /// </summary>
    public string GameAlias { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the game, if available.
    /// </summary>
    public string? Description { get; set; }
}
