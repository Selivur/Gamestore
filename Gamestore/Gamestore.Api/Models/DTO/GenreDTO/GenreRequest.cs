using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.GenreDTO;

public class GenreRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    public int? ParentId { get; set; }
}
