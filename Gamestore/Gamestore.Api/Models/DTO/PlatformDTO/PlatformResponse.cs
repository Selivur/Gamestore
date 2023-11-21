using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.PlatformDTO;

public class PlatformResponse
{
    public int Id { get; set; }

    public string Type { get; set; }

    public static PlatformResponse FromPlatform(Platform platform)
    {
        return new PlatformResponse
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }
}
