namespace Gamestore.Api.Models.DTO;

public class BanRequest
{
    /// <summary>
    /// Gets or sets the name of the user to ban.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the duration of the ban.
    /// </summary>
    public string Duration { get; set; }
}
