using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Provides the methods to process payments.
/// </summary>
public class PaymentService : IPaymentService
{
    private const string ApiUrl = "https://localhost:5001/api/payments";

    private readonly IOrderRepository _unitOfWork;

    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The order repository.</param>
    /// <param name="httpClient">The http client.</param>
    public PaymentService(IOrderRepository unitOfWork, HttpClient httpClient)
    {
        _unitOfWork = unitOfWork;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IBoxResponseDTO> ProcessIboxPayment()
    {
        var order = await _unitOfWork.GetFirstOpenOrderAsync();

        var iboxTransaction = new IBoxTransactionDTO
        {
            TransactionAmount = order.OrderDetails.Sum(details => details.Price),
            AccountNumber = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            InvoiceNumber = Guid.NewGuid().ToString(),
        };

        var responce = await _httpClient.PostAsJsonAsync($"{ApiUrl}/ibox", iboxTransaction);

        if (responce.IsSuccessStatusCode)
        {
            await _unitOfWork.CompleteOrder();
        }
        else
        {
            await _unitOfWork.CancelledOrder();
        }

        var result = new IBoxResponseDTO
        {
            OrderId = order.Id,
            UserId = order.Customer.Id,
            Sum = order.OrderDetails.Sum(details => details.Price),
        };

        return result;
    }

    /// <inheritdoc/>
    public async Task<IBoxResponseDTO> ProcessVisaPayment(VisaTransactionDTO model)
    {
        var order = await _unitOfWork.GetFirstOpenOrderAsync();

        var responce = await _httpClient.PostAsJsonAsync($"{ApiUrl}/visa", model);

        if (responce.IsSuccessStatusCode)
        {
            await _unitOfWork.CompleteOrder();
        }
        else
        {
            await _unitOfWork.CancelledOrder();
        }

        var result = new IBoxResponseDTO
        {
            OrderId = order.Id,
            UserId = order.Customer.Id,
            Sum = order.OrderDetails.Sum(details => details.Price),
        };

        return result;
    }
}
