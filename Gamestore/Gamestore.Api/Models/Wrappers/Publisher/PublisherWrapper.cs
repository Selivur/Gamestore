using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PublisherDTO;

namespace Gamestore.Api.Models.Wrappers.Publisher;

/// <summary>
/// Represents a wrapper for a publisher, including publisher information.
/// </summary>
public class PublisherWrapper
{
    /// <summary>
    /// Gets or sets the publisher information.
    /// </summary>
    [JsonPropertyName("publisher")]
    public PublisherRequest PublisherRequest { get; set; }
}
