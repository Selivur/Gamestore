using Gamestore.Api.Models.DTO.GenreDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;
[ApiController]
[Route("genres")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreController"/> class.
    /// </summary>
    /// <param name="genreService">The service for managing genres.</param>
    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// <summary>
    /// Retrieves a genre by its id.
    /// </summary>
    /// <param name="id">The id of the game to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved game if successful.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenre(int id)
    {
        var result = await _genreService.GetGenreByIdAsync(id);

        return Ok(result);
    }

    /// <summary>
    /// Updates the description of a genre through the API.
    /// </summary>
    /// <param name="request">The request object containing the identifier and updated data for the genre.</param>
    /// <returns>
    /// 200 OK if the update is successful.
    /// 400 Bad Request if the model state is invalid.
    /// </returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateGenre([FromBody] GenreUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _genreService.UpdateGenreAsync(request);

        return Ok($"Genre with ID {request.Id} updated successfully");
    }

    /// <summary>
    /// Deletes a genre through the API.
    /// </summary>
    /// <param name="name">The name of the genre to be removed.</param>
    /// <returns>A 204 No Content response upon successful deletion.</returns>
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveGenre(string name)
    {
        await _genreService.RemoveGenreAsync(name);

        return NoContent();
    }

    /// <summary>
    /// Creates a new genre in the database.
    /// </summary>
    /// <param name="genre">The genre object to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> AddGenre([FromBody] GenreRequest genre)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _genreService.AddGenreAsync(genre);

        return Ok();
    }

    /// <summary>
    /// Initializes predefined genres through the API.
    /// </summary>
    /// <returns>
    /// 200 OK if the initialization is successful.
    /// </returns>
    [HttpPost("System_generate")]
    public async Task<IActionResult> InitPredefinedGenres()
    {
        await _genreService.InitializePredefinedGenresAsync();
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
