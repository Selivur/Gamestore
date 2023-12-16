using Gamestore.Api.Models.DTO.PublisherDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;

/// <summary>
/// API controller for managing publisher-related operations.
/// </summary>
[Route("publishers")]
[ApiController]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherController"/> class.
    /// </summary>
    /// <param name="publisherService">The publisher service used for handling publisher-related operations.</param>
    public PublisherController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    /// <summary>
    /// Retrieves a publisher by its id.
    /// </summary>
    /// <param name="id">The id of the publisher to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved publisher if successful.</returns>
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetPublisher(int id)
    {
        var result = await _publisherService.GetPublisherByIdAsync(id);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new publisher in the database.
    /// </summary>
    /// <param name="publisher">The publisher object to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> CreatePublisher([FromBody] PublisherRequest publisher)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _publisherService.CreatePublisherAsync(publisher);

        return Ok();
    }

    /// <summary>
    /// Retrieves a publisher by its name.
    /// </summary>
    /// <param name="publisherName">The name of the publisher to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved publisher if successful.</returns>
    [HttpGet("getByPublisherName/{publisherName}")]
    public async Task<IActionResult> GetPublisher(string publisherName)
    {
        var result = await _publisherService.GetPublisherByNameAsync(publisherName);

        return Ok(result);
    }

    /// <summary>
    /// Updates a publisher in the publisher store.
    /// </summary>
    /// <param name="publisher">The publisher object to update.</param>
    /// <returns>An IActionResult object representing the result of the update operation.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdatePublisher([FromBody] PublisherRequest publisher)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _publisherService.UpdatePublisherAsync(publisher);

        return Ok();
    }

    /// <summary>
    /// Removes a publisher from the store by its name.
    /// </summary>
    /// <param name="id">The id of the publisher to remove.</param>
    /// <returns>A 204 No Content response upon successful deletion.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> RemovePublisher(int id)
    {
        await _publisherService.RemovePublisherAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Retrieves all publishers.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of publishers.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPublishers()
    {
        var publishers = await _publisherService.GetAllPublishersAsync();

        return Ok(publishers);
    }

    /// <summary>
    /// Retrieves a list of publishers associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to filter publishers.</param>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of publishers.
    /// </returns>
    [HttpGet("getPublisherByGameKey/{gameAlias}")]
    public async Task<IActionResult> GetAllPublishersByGameAlias(string gameAlias)
    {
        var publishers = await _publisherService.GetPublishersByGameAliasAsync(gameAlias);

        return Ok(publishers);
    }

    /// <summary>
    /// Returns a list of error messages from the ModelState object.
    /// </summary>
    /// <returns>A list of error messages.</returns>
    private List<string> GetErrorMessages()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(v => v.ErrorMessage)
            .ToList();
    }
}