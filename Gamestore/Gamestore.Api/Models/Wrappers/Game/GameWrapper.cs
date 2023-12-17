using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.GameDTO;

namespace Gamestore.Api.Models.Wrappers.Game;

public class GameWrapper
{
    [JsonPropertyName("game")]
    public GameRequest GameRequest { get; set; }

    [JsonPropertyName("genres")]
    public int[] GenresId { get; set; }

    [JsonPropertyName("platforms")]
    public int[] PlatformsId { get; set; }

    [JsonPropertyName("publisher")]
    public int PublisherId { get; set; }
}