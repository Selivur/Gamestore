namespace Gamestore.Database.Entities;
public class Order
{
    public int Id { get; set; }

    public DateTime? OrderDate { get; set; }

    public int Price { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; }

    public Customer Customer { get; set; }
}
