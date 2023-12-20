namespace Gamestore.Database.Entities;
public class OrderDetails
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public Game Game { get; set; }
}
