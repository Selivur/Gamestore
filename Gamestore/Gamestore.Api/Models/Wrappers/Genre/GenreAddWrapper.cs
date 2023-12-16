using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GenreDTO;

namespace Gamestore.Api.Models.Wrappers.Genre;

public class GenreAddWrapper
{
    [JsonPropertyName("genre")]
    public GenreRequest GenreRequsert { get; set; }
}
