﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;

/// <summary>
/// Represents a game entity in the Gamestore database.
/// </summary>
[Index(nameof(GameAlias), IsUnique = true)]
public class Game
{
    /// <summary>
    /// Gets or sets the unique identifier of the game.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the game alias.
    /// </summary>
    public string GameAlias { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the price of the game.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the available units in stock.
    /// </summary>
    public int UnitInStock { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the game.
    /// </summary>
    public int Discount { get; set; }

    /// <summary>
    /// Gets or sets the description of the game.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of genres associated with the game.
    /// </summary>
    public ICollection<Genre> Genre { get; set; }

    /// <summary>
    /// Gets or sets the collection of platforms associated with the game.
    /// </summary>
    public ICollection<Platform> Platforms { get; set; }

    /// <summary>
    /// Gets or sets the publisher associated with the game.
    /// </summary>
    public Publisher? Publishers { get; set; }
}
