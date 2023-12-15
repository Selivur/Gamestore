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

    public int Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }

    public string? Description { get; set; }

    public ICollection<Genre>? Genre { get; set; }

    public ICollection<Platform>? Platforms { get; set; }

    public Publisher? Publishers { get; set; }
}