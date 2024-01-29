namespace Gamestore.Api.Models.DTO.OrderDTO;

public class IBoxTransactionDTO
{
    public decimal TransactionAmount { get; set; }

    public string AccountNumber { get; set; }

    public string InvoiceNumber { get; set; }
}
