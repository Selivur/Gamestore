using Gamestore.Api.Models.DTO.PlatformDTO;

namespace Gamestore.Api.Services.Interfaces;

/// <summary>
/// Service for managing platforms.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets all platforms.
    /// </summary>
    /// <returns>The collection of all platforms.</returns>
    Task<IEnumerable<PlatformResponse>> GetAllPlatformsAsync();

    /// <summary>
    /// Adds a new platform.
    /// </summary>
    /// <param name="name">Name of new platform.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddPlatformAsync(string name);

    /// <summary>
    /// Gets a platform by its name.
    /// </summary>
    /// <param name="name">The name of the platform.</param>
    /// <returns>The detailed information about the platform.</returns>
    Task<PlatformResponse?> GetPlatformByNameAsync(string name);

    /// <summary>
    /// Gets a platform by its id.
    /// </summary>
    /// <param name="id">The id of the platform.</param>
    /// <returns>The detailed information about the platform.</returns>
    Task<PlatformResponse?> GetPlatformByIdAsync(int id);

    /// <summary>
    /// Updates a platform by its identifier.
    /// </summary>
    /// <param name="platform">The request object containing the identifier and updated data for the platform.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdatePlatformAsync(PlatformRequest platform);

    /// <summary>
    /// Removes a platform by its id.
    /// </summary>
    /// <param name="id">The id of the platform to be removed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemovePlatformAsync(int id);

    /// <summary>
    /// Gets a list of platforms in the form of <see cref="PlatformResponse"/>.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to filter platforms.</param>
    /// <returns>An asynchronous task representing the operation, returning a list of <see cref="PlatformResponse"/>.</returns>
    Task<IEnumerable<PlatformResponse>> GetPlatformsByGameAliasAsync(string gameAlias);
}