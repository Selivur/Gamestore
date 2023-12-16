using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;
[Index(nameof(Name), IsUnique = true)]
public class Genre
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public Genre ParentGenre { get; set; }

    public ICollection<Game> Games { get; set; }
}
