namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object (DTO) for payment details.
/// </summary>
public class PaymentDetails
{
    /// <summary>
    /// Gets or sets the title of the payment details.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the image URL associated with the payment details.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the description of the payment details.
    /// </summary>
    public string Description { get; set; }
}
