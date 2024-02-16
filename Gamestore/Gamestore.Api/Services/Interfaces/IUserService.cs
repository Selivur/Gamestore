namespace Gamestore.Api.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Returns the possible durations for a ban.
    /// </summary>
    /// <returns>An array of strings representing the possible ban durations.</returns>
    public Task<string[]> GetBanDurationsAsync();

    /// <summary>
    /// Bans a user.
    /// </summary>
    /// <param name="userName">The name of the user to ban.</param>
    /// <param name="banDuration">The duration of the ban.</param>
    public Task BanUserAsync(string userName, string banDuration);
}