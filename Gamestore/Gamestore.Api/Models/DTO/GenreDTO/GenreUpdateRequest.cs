using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.GenreDTO;

public class GenreUpdateRequest
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    public int? ParentId { get; set; }
}