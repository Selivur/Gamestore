namespace Gamestore.Database.Entities;

public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    public ICollection<Genre> Genres { get; set; }
}
