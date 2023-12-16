using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Entities;
[Index(nameof(CompanyName), IsUnique = true)]
public class Publisher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string CompanyName { get; set; }

    public string Description { get; set; }

    public string HomePage { get; set; }

    public ICollection<Game> Games { get; set; }
}
