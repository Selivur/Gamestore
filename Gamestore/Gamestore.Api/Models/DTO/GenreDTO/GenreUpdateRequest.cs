using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.GenreDTO;

public class GenreUpdateRequest
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    public GenreRequest Genre { get; set; }
}