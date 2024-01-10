using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.OrderDTO;

public class CartDetailsDTO
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public string GameId { get; set; }

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
