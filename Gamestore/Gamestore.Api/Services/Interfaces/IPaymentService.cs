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
}