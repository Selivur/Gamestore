using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.PublisherDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a publisher request.
/// </summary>
public class PublisherRequest
{
    /// <summary>
    /// Gets or sets the company name of the publisher. Required.
    /// </summary>
    [StringLength(40, MinimumLength = 1, ErrorMessage = "The Company Name must be a string with a minimum length of 1 and a maximum length of 40.")]
    [Required(ErrorMessage = "Company Name is required")]
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the description of the publisher. Required.
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the home page URL of the publisher. Required.
    /// </summary>
    [Required(ErrorMessage = "Home Page is required")]
    public string HomePage { get; set; }
}
