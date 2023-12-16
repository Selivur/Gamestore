using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GenreDTO;

namespace Gamestore.Api.Models.Wrappers.Genre;

public class GenreUpdateWrapper
{
    [JsonPropertyName("genre")]
    public GenreUpdateRequest GenreRequsert { get; set; }
}
