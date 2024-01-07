namespace Gamestore.Api.Models.DTO.OrderDTO;

public class OrderRequest
{
    public string Id { get; set; }

    public string CustomerId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public int Sum { get; set; }

    public DateTime? OrderDate { get; set; }

    public int Price { get; set; }

    public int Discount { get; set; }
}
