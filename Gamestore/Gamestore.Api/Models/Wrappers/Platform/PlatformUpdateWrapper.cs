using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PlatformDTO;

namespace Gamestore.Api.Models.Wrappers.Platform;

/// <summary>
/// Represents a wrapper for updating a platform, including platform update information.
/// </summary>
public class PlatformUpdateWrapper
{
    /// <summary>
    /// Gets or sets the platform update information.
    /// </summary>
    [JsonPropertyName("platform")]
    public PlatformRequest PlatformRequest { get; set; }
}
