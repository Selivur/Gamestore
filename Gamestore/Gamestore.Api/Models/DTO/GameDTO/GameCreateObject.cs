namespace Gamestore.Api.Models.DTO.GameDTO;

public class GameCreateObject
{
    public GameRequest GameRequest { get; set; }

    public int[] GenresId { get; set; }

    public int[] PlatformsId { get; set; }

    public int PublisherId { get; set; }
}
