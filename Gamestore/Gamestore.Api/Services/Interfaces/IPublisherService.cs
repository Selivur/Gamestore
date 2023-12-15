using Gamestore.Api.Models.DTO.PublisherDTO;

namespace Gamestore.Api.Services.Interfaces;

public interface IPublisherService
{
    /// <summary>
    /// Creates a new Publisher in the database.
    /// </summary>
    /// <param name="publisher">The Publisher object to be created.</param>
    Task CreatePublisherAsync(PublisherRequest publisher);

    /// <summary>
    /// Gets a Publisher by its name.
    /// </summary>
    /// <param name="id">The name of the Publisher.</param>
    /// <returns>The detailed information about the Publisher.</returns>
    Task<PublisherResponse?> GetPublisherByIdAsync(int id);

    /// <summary>
    /// Retrieves a Publisher from the database by its name.
    /// </summary>
    /// <param name="name">The name of the Publisher to retrieve.</param>
    /// <returns>The retrieved Publisher object.</returns>
    /// <remarks>If the Publisher name is null or empty, the method will return a tuple with a false success value and an error message. If the Publisher is not found in the database, the method will return a tuple with a false success value and an error message. Otherwise, the method will return a tuple with a true success value and the retrieved Publisher object.</remarks>
    Task<PublisherResponse?> GetPublisherByNameAsync(string name);

    /// <summary>
    /// Updates an existing Publisher in the database.
    /// </summary>
    /// <param name="publisher">The Publisher object containing the updated properties.</param>
    Task UpdatePublisherAsync(PublisherRequest publisher);

    /// <summary>
    /// Removes a Publisher from the database based on its name.
    /// </summary>
    /// <param name="name">The name of the Publisher to be removed.</param>
    /// <returns>A tuple indicating whether the removal was successful and an error message if applicable.</returns>
    Task RemovePublisherAsync(string name);

    /// <summary>
    /// Retrieves all publishers from the database.
    /// </summary>
    /// <returns>An IEnumerable of Publisher objects.</returns>
    Task<IEnumerable<PublisherResponse>> GetAllPublishersAsync();
}