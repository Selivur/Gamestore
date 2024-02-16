using Gamestore.Api.Services.Interfaces;

namespace Gamestore.Api.Services;

public class UserService : IUserService
{
    /// <inheritdoc/>
    public Task BanUserAsync(string userName, string banDuration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<string[]> GetBanDurationsAsync()
    {
        return Task.FromResult(new string[] { "1 hour", "1 day", "1 week", "1 month", "permanent" });
    }
}
