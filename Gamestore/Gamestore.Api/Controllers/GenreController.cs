using Gamestore.Api.Models.Wrappers.Genre;
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
    /// <param name="id">The id of the genre to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved genre if successful.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenre(int id)
    {
        var result = await _genreService.GetGenreByIdAsync(id);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves genres by their parent ID.
    /// </summary>
    /// <param name="id">The ID of the parent genre.</param>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of genres.
    /// </returns>
    [HttpGet("getByParentGenre/{Id}")]
    public async Task<IActionResult> GetGenresByParentId(int id)
    {
        var genres = await _genreService.GetGenresByParentIdAsync(id);

        return Ok(genres);
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
    public async Task<IActionResult> UpdateGenre([FromBody] GenreUpdateWrapper request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _genreService.UpdateGenreAsync(request.GenreRequest);

        return Ok();
    }

    /// <summary>
    /// Deletes a genre through the API.
    /// </summary>
    /// <param name="id">The id of the genre to be removed.</param>
    /// <returns>A 204 No Content response upon successful deletion.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> RemoveGenre(int id)
    {
        await _genreService.RemoveGenreAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Creates a new genre in the database.
    /// </summary>
    /// <param name="genre">The genre object to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> AddGenre([FromBody] GenreAddWrapper genre)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _genreService.AddGenreAsync(genre.GenreRequest);

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
    /// Retrieves all genre.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of genres.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();

        return Ok(genres);
    }

    /// <summary>
    /// Retrieves a list of genres associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to filter genres.</param>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of genres.
    /// </returns>
    [HttpGet("getGenresByGameKey/{gameAlias}")]
    public async Task<IActionResult> GetAllGenresByGameAlias(string gameAlias)
    {
        var genres = await _genreService.GetGenresByGameAliasAsync(gameAlias);

        return Ok(genres);
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
