using System.Text.Json.Serialization;
using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.GameDTO;

/// <summary>
/// Represents a response containing game information.
/// </summary>
public class GameResponse
{
    /// <summary>
    /// Gets or sets the alias of the game.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the game.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the game, if available.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the game.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the available units in stock for the game.
    /// </summary>
    public int UnitInStock { get; set; }

    /// <summary>
    /// Gets or sets the Discontinued applied to the game.
    /// </summary>
    [JsonPropertyName("discontinued")]
    public int Discount { get; set; }

    /// <summary>
    /// Converts a Game object to a GameResponse object.
    /// </summary>
    /// <param name="game">The Game object to convert.</param>
    /// <returns>The converted GameResponse object.</returns>
    public static GameResponse FromGame(Game game)
    {
        return new GameResponse
        {
            Key = game.GameAlias,
            Id = game.Id.ToString(),
            Name = game.Name,
            Description = game.Description,
            Price = game.Price,
            UnitInStock = game.UnitInStock,
            Discount = game.Discount,
        };
    }
}
