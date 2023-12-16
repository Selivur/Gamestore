using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PlatformDTO;

namespace Gamestore.Api.Models.Wrappers.Platform;

public class PlatformUpdateWrapper
{
    [JsonPropertyName("platform")]
    public PlatformRequest PlatformRequest { get; set; }
}
