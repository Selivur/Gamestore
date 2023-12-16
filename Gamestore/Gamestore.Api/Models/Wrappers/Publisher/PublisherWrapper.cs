using System.Text.Json.Serialization;
using Gamestore.Api.Models.DTO.PublisherDTO;

namespace Gamestore.Api.Models.Wrappers.Publisher;

public class PublisherWrapper
{
    [JsonPropertyName("publisher")]
    public PublisherRequest PublisherRequest { get; set; }
}
