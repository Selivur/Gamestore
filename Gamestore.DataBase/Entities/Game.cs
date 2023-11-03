using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;
[Index(nameof(GameAlias), IsUnique = true)]
public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string GameAlias { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public Genre? Genre { get; set; }

    public Platform? Platforms { get; set; }
}