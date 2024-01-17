using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GenreDTO;

namespace Gamestore.Api.Models.Wrappers.Genre;

/// <summary>
/// Represents a wrapper for updating a genre, including genre update information.
/// </summary>
public class GenreUpdateWrapper
{
    /// <summary>
    /// Gets or sets the genre update information.
    /// </summary>
    [JsonPropertyName("genre")]
    public GenreUpdateRequest GenreRequest { get; set; }
}
