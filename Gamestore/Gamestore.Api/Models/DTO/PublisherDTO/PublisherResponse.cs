using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.PublisherDTO;

/// <summary>
/// Represents a data transfer object (DTO) for a publisher response.
/// </summary>
public class PublisherResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the publisher.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the company name of the publisher.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the description of the publisher.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the home page URL of the publisher.
    /// </summary>
    public string HomePage { get; set; }

    /// <summary>
    /// Converts a <see cref="Publisher"/> object to a <see cref="PublisherResponse"/> object.
    /// </summary>
    /// <param name="publisher">The <see cref="Publisher"/> object to convert.</param>
    /// <returns>A new instance of <see cref="PublisherResponse"/> populated with data from the input <paramref name="publisher"/>.</returns>
    public static PublisherResponse FromPublisher(Publisher publisher)
    {
        return new PublisherResponse
        {
            Id = publisher.Id.ToString(),
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        };
    }
}
