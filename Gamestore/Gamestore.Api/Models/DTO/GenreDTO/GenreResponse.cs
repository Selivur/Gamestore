using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.GenreDTO;

public class GenreResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    public static GenreResponse FromGenre(Genre genre)
    {
        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentId = genre.ParentId,
        };
    }
}
