using Gamestore.Api.Models.DTO.PlatformDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;

[ApiController]
[Route("platforms")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformController"/> class.
    /// </summary>
    /// <param name="platformService">The service for managing platforms.</param>
    public PlatformController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    /// <summary>
    /// Retrieves a platform by its id.
    /// </summary>
    /// <param name="id">The id of the platform to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved platform if successful.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatform(int id)
    {
        var result = await _platformService.GetPlatformByIdAsync(id);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all platforms.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of platforms.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPlatforms()
    {
        var platforms = await _platformService.GetAllPlatformsAsync();

        return Ok(platforms);
    }

    /// <summary>
    /// Retrieves a list of platforms associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to filter platforms.</param>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of platforms.
    /// </returns>
    [HttpGet("getPlatformsByGameKey/{gameAlias}")]
    public async Task<IActionResult> GetAllPlatformsByGameAlias(string gameAlias)
    {
        var platforms = await _platformService.GetPlatformsByGameAliasAsync(gameAlias);

        return Ok(platforms);
    }

    /// <summary>
    /// Updates the description of a platform through the API.
    /// </summary>
    /// <param name="request">The request object containing the identifier and updated data for the platform.</param>
    /// <returns>
    /// 200 OK if the update is successful.
    /// 400 Bad Request if the model state is invalid.
    /// </returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdatePlatform([FromBody] PlatformRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _platformService.UpdatePlatformAsync(request);

        return Ok($"Platform with ID {request.Id} updated successfully");
    }

    /// <summary>
    /// Deletes a platform through by id.
    /// </summary>
    /// <param name="id">The id of the platform to be removed.</param>
    /// <returns>A 204 No Content response upon successful deletion.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> RemovePlatform(int id)
    {
        await _platformService.RemovePlatformAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Creates a new platform in the database.
    /// </summary>
    /// <param name="name">The platform name to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> AddPlatform([FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        await _platformService.AddPlatformAsync(name);

        return Ok();
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
