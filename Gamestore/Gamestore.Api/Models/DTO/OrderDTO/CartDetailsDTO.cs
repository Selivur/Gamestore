using System.Text.Json.Serialization;
using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.OrderDTO;

/// <summary>
/// Represents a data transfer object (DTO) for cart details.
/// </summary>
public class CartDetailsDTO
{
    /// <summary>
    /// Gets or sets the unique identifier of the cart details.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the quantity of items in the cart.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price of the items in the cart.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated game.
    /// </summary>
    [JsonPropertyName("productId")]
    public string GameId { get; set; }

    /// <summary>
    /// Converts an <see cref="OrderDetails"/> object to a <see cref="CartDetailsDTO"/> object.
    /// </summary>
    /// <param name="orderDetails">The <see cref="OrderDetails"/> object to convert.</param>
    /// <returns>A new instance of <see cref="CartDetailsDTO"/> populated with data from the input <paramref name="orderDetails"/>.</returns>
    public static CartDetailsDTO FromOrderDetails(OrderDetails orderDetails)
    {
        return new CartDetailsDTO
        {
            Id = orderDetails.Id,
            Quantity = orderDetails.Quantity,
            Price = orderDetails.Price,
            GameId = orderDetails.Game.Id.ToString(),
        };
    }
}