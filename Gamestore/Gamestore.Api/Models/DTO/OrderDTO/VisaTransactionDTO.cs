using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object for a Visa transaction.
/// </summary>
public class VisaTransactionDTO
{
    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Gets or sets the cardholder's name.
    /// </summary>
    public string Holder { get; set; }

    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    [MinLength(1)]
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card's expiration month.
    /// </summary>
    [Range(1, 12)]
    public int MonthExpire { get; set; }

    /// <summary>
    /// Gets or sets the card's CVV2 (Card Verification Value 2) code.
    /// </summary>
    [Range(1, 999)]
    public int CVV2 { get; set; }

    /// <summary>
    /// Gets or sets the card's expiration year.
    /// </summary>
    [Range(1960, 2147483647)]
    public int YearExpire { get; set; }
}