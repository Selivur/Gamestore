using Gamestore.Api.Models.DTO.PublisherDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Implementation of the <see cref="IPublisherService"/> interface.
/// Provides functionality for managing publisher-related operations.
/// </summary>
public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherService"/> class.
    /// </summary>
    /// <param name="repository">The publisher repository providing data access for the service.</param>
    public PublisherService(IPublisherRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task CreatePublisherAsync(PublisherRequest publisher)
    {
        var existingPublisher = await _repository.GetByNameAsync(publisher.CompanyName);

        if (existingPublisher != null)
        {
            throw new InvalidOperationException("Publisher name must be unique");
        }

        Publisher newPublisher = new()
        {
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        };

        await _repository.AddAsync(newPublisher);
    }

    /// <inheritdoc/>
    public async Task<PublisherResponse?> GetPublisherByNameAsync(string name)
    {
        var publisher = await _repository.GetByNameAsync(name) ?? throw new KeyNotFoundException("Publisher not found");

        return PublisherResponse.FromPublisher(publisher);
    }

    /// <inheritdoc/>
    public async Task UpdatePublisherAsync(PublisherRequest publisher)
    {
        var existingPublisher = await _repository.GetByNameAsync(publisher.CompanyName) ?? throw new KeyNotFoundException("Can't find the Publisher with this Company name");
        existingPublisher.CompanyName = publisher.CompanyName;
        existingPublisher.Description = publisher.Description;
        existingPublisher.HomePage = publisher.HomePage;

        await _repository.UpdateAsync(existingPublisher);
    }

    /// <inheritdoc/>
    public async Task RemovePublisherAsync(string name)
    {
        await _repository.RemoveAsync(name);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PublisherResponse>> GetAllPublishersAsync()
    {
        var publishers = await _repository.GetAllAsync();

        var publisherResponses = publishers.Select(PublisherResponse.FromPublisher).ToList();

        return publisherResponses;
    }
}
