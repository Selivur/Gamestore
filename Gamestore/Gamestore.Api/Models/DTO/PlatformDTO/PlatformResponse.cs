using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.PlatformDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a platform response.
/// </summary>
public class PlatformResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the platform.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the platform.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Converts a <see cref="Platform"/> object to a <see cref="PlatformResponse"/> object.
    /// </summary>
    /// <param name="platform">The <see cref="Platform"/> object to convert.</param>
    /// <returns>A new instance of <see cref="PlatformResponse"/> populated with data from the input <paramref name="platform"/>.</returns>
    public static PlatformResponse FromPlatform(Platform platform)
    {
        return new PlatformResponse
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }
}
