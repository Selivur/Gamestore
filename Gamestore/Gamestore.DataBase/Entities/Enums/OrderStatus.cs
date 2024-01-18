namespace Gamestore.Database.Entities.Enums;

/// <summary>
/// Represents the possible statuses of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// The order is open and awaiting processing.
    /// </summary>
    Open,

    /// <summary>
    /// The order is in the checkout process.
    /// </summary>
    Checkout,

    /// <summary>
    /// The order has been paid for.
    /// </summary>
    Paid,

    /// <summary>
    /// The order has been shipped.
    /// </summary>
    Shipped,

    /// <summary>
    /// The order has been cancelled.
    /// </summary>
    Cancelled,
}
