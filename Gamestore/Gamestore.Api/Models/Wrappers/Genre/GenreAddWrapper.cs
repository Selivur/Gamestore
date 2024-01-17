using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GenreDTO;

namespace Gamestore.Api.Models.Wrappers.Genre;

/// <summary>
/// Represents a wrapper for adding a genre, including genre information.
/// </summary>
public class GenreAddWrapper
{
    /// <summary>
    /// Gets or sets the genre information.
    /// </summary>
    [JsonPropertyName("genre")]
    public GenreRequest GenreRequest { get; set; }
}
