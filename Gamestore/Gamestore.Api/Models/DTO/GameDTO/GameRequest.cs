﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gamestore.Api.Models.DTO.GameDTO;

/// <summary>
/// Represents a request for creating or updating a game.
/// </summary>
public class GameRequest
{
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers allowed in the id field.")]
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

    /// <summary>
    /// Gets or sets the discount of the game.
    /// </summary>
    [JsonPropertyName("discontinued")]
    public int Discount { get; set; }

    /// <summary>
    /// Gets or sets the price of the game.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the available quantity of the game.
    /// </summary>
    public int UnitInStock { get; set; }

    /// <summary>
    /// Converts a Game object to a GameResponse object.
    /// </summary>
    /// <param name="game">The Game object to convert.</param>
    /// <returns>The converted GameResponse object.</returns>
    public static GameRequest FromGameResponse(GameResponse game)
    {
        return new GameRequest
        {
            GameAlias = game.Key,
            Id = game.Id.ToString(),
            Name = game.Name,
            Description = game.Description ?? string.Empty,
            Price = game.Price,
            UnitInStock = game.UnitInStock,
            Discount = game.Discount,
        };
    }
}