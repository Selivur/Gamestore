using Gamestore.Database.Entities;

namespace Gamestore.Api.Models.DTO.PublisherDTO;

public class PublisherResponse
{
    public string Id { get; set; }

    public string CompanyName { get; set; }

    public string Description { get; set; }

    public string HomePage { get; set; }

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
