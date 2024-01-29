namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Data transfer object for encapsulating response data of orders paid using iBox terminal.
/// </summary>
public class IBoxResponseDTO
{
    /// <summary>
    /// Gets or sets the identifier of the order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the sum of the order.
    /// </summary>
    public int Sum { get; set; }
}
