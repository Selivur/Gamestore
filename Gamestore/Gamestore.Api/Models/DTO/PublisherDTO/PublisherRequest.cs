using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.PublisherDTO;

public class PublisherRequest
{
    [StringLength(40, MinimumLength = 1, ErrorMessage = "The Company Name must be a string with a minimum length of 1 and a maximum length of 40.")]
    [Required(ErrorMessage = "Company Name is required")]
    public string CompanyName { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Home Page is required")]
    public string HomePage { get; set; }
}
