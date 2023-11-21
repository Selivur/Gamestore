using Gamestore.Api.Models.DTO.PlatformDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Service for managing platforms.
/// </summary>
public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformService"/> class.
    /// </summary>
    /// <param name="platformRepository">The repository for platforms.</param>
    public PlatformService(IPlatformRepository platformRepository)
    {
        _repository = platformRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <inheritdoc/>
    public async Task AddPlatformAsync(string name)
    {
        var existingPlatform = await _repository.GetByNameAsync(name);

        if (existingPlatform != null)
        {
            throw new InvalidOperationException("Platform name must be unique");
        }

        Platform newPlatform = new()
        {
            Type = name,
        };

        await _repository.AddAsync(newPlatform);
    }

    /// <inheritdoc/>
    public async Task<PlatformResponse?> GetPlatformByNameAsync(string name)
    {
        var platform = await _repository.GetByNameAsync(name) ?? throw new KeyNotFoundException("Platform not found");

        return PlatformResponse.FromPlatform(platform);
    }

    /// <inheritdoc/>
    public async Task<PlatformResponse?> GetPlatformByIdAsync(int id)
    {
        var platform = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Platform not found");

        return PlatformResponse.FromPlatform(platform);
    }

    /// <inheritdoc/>
    public async Task UpdatePlatformAsync(PlatformRequest platform)
    {
        var existingPlatform = await _repository.GetByIdAsync(platform.Id) ?? throw new KeyNotFoundException("Can't find the Platform with this id");

        existingPlatform.Type = platform.Type;

        await _repository.UpdateAsync(existingPlatform);
    }

    /// <inheritdoc/>
    public async Task RemovePlatformAsync(string name)
    {
        await _repository.RemoveAsync(name);
    }
}
