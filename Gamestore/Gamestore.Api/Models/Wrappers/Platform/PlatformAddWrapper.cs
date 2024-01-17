using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PlatformDTO;

namespace Gamestore.Api.Models.Wrappers.Platform;

/// <summary>
/// Represents a wrapper for adding a platform, including platform type information.
/// </summary>
public class PlatformAddWrapper
{
    /// <summary>
    /// Gets or sets the platform type information.
    /// </summary>
    [JsonPropertyName("platform")]
    public PlatformTypeRequest PlatformTypeRequest { get; set; }
}
