using System.ComponentModel.DataAnnotations;

namespace Gamestore.Database.Entities;

public class Game
{
    [Key]
    [Required(ErrorMessage = "Game Alias is required")]
    public string GameAlias { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    public string? Description { get; set; }

    public Genre? Genre { get; set; }

    public Platform? Platforms { get; set; }
}