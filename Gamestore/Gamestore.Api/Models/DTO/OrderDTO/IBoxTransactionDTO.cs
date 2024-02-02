namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents data for an IBox transaction.
/// </summary>
public class IBoxTransactionDTO
{
    /// <summary>
    /// Gets or sets the transaction amount for the IBox transaction.
    /// </summary>
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Gets or sets the account number associated with the IBox transaction.
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the invoice number for the IBox transaction.
    /// </summary>
    public string InvoiceNumber { get; set; }
}
