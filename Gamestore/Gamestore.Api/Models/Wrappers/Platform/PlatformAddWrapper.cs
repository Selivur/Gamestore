using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PlatformDTO;

namespace Gamestore.Api.Models.Wrappers.Platform;

public class PlatformAddWrapper
{
    [JsonPropertyName("platform")]
    public PlatformTypeRequest PlatformTypeRequest { get; set; }
}