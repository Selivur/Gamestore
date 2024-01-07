using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.OrderDTO;

public class OrderResponse
{
    public int Id { get; set; }

    public DateTime? OrderDate { get; set; }

    public string CustomerId { get; set; }

    public static OrderResponse FromOrder(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.Customer.Id.ToString(),
        };
    }
}
