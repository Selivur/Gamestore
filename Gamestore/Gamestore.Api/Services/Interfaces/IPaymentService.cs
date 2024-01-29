using Gamestore.Api.Models.DTO.OrderDTO;

namespace Gamestore.Api.Services.Interfaces;

/// <summary>
/// Defines the operations for processing payments.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes ibox payment for an order.
    /// </summary>
    /// <returns>The details of processed ibox payment encapsulated in IboxResponseDTO object.</returns>
    Task<IBoxResponseDTO> ProcessIboxPayment();

    /// <summary>
    /// Processes a Visa payment for an order.
    /// </summary>
    /// <param name="model">The VisaTransactionDTO object containing the transaction data.</param>
    /// <returns>Details of the processed Visa payment encapsulated in an IBoxResponseDTO object.</returns>
    Task<IBoxResponseDTO> ProcessVisaPayment(VisaTransactionDTO model);
}