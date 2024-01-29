namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object for processing a payment request.
/// </summary>
public class PaymentRequestDTO
{
    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the Visa transaction data transfer object.
    /// </summary>
    public VisaTransactionDTO? Model { get; set; }
}
