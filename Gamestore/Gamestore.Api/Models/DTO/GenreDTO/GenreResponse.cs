using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.GenreDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a genre response.
/// </summary>
public class GenreResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the genre.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional parent ID of the genre.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Converts a <see cref="Genre"/> object to a <see cref="GenreResponse"/> object.
    /// </summary>
    /// <param name="genre">The <see cref="Genre"/> object to convert.</param>
    /// <returns>A new instance of <see cref="GenreResponse"/> populated with data from the input <paramref name="genre"/>.</returns>
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