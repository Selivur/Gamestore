using System.Text;
using Gamestore.Api.Models.DTO;
using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Models.Wrappers.Game;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;

/// <summary>
/// API controller for managing game-related operations.
/// </summary>
[Route("games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="gameService">The game service used for handling game-related operations.</param>
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Retrieves a game by its id.
    /// </summary>
    /// <param name="id">The id of the game to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved game if successful.</returns>
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetGenre(string id)
    {
        var result = await _gameService.GetGameByIdAsync(Convert.ToInt32(id));

        return Ok(result);
    }

    /// <summary>
    /// Creates a new game in the database.
    /// </summary>
    /// <param name="game">The game object to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> CreateGame([FromBody] GameWrapper game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _gameService.CreateGameAsync(game);

        return Ok();
    }

    /// <summary>
    /// Retrieves a game by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to retrieve.</param>
    /// <returns>An IActionResult containing the retrieved game if successful.</returns>
    [HttpGet("getByAlias/{gameAlias}")]
    public async Task<IActionResult> GetGame(string gameAlias)
    {
        var result = await _gameService.GetGameResponseByAliasAsync(gameAlias);

        return Ok(result);
    }

    /// <summary>
    /// Updates a game in the game store.
    /// </summary>
    /// <param name="game">The game object to update.</param>
    /// <returns>An IActionResult object representing the result of the update operation.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateGame([FromBody] GameWrapper game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GetErrorMessages());
        }

        await _gameService.UpdateGameAsync(game);

        return Ok();
    }

    /// <summary>
    /// Removes a game from the store by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to remove.</param>
    /// <returns>A 204 No Content response upon successful deletion.</returns>
    [HttpDelete("remove/{gameAlias}")]
    public async Task<IActionResult> RemoveGame(string gameAlias)
    {
        await _gameService.RemoveGameAsync(gameAlias);

        return NoContent();
    }

    /// <summary>
    /// Downloads a game file for the specified game alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to download.</param>
    /// <returns>The game file as a downloadable file.</returns>
    [HttpGet("{gameAlias}/download")]
    public async Task<IActionResult> DownloadGame(string gameAlias)
    {
        var game = await _gameService.GetGameResponseByAliasAsync(gameAlias);

        var content = GenerateGameFileContent(game);
        var fileName = $"{game.Name}_{DateTime.Now:yyyyMMddHHmmss}.txt";
        var contentType = "text/plain";
        var bytes = Encoding.UTF8.GetBytes(content);

        return File(bytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves all games from the game store.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with the list of games.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();

        return Ok(games);
    }

    /// <summary>
    /// Asynchronously gets all games by publisher name.
    /// </summary>
    /// <param name="name">The name of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult that encapsulates the result of the action method.</returns>
    [HttpGet("publisher/{name}")]
    public async Task<IActionResult> GetAllGamesByPublisherName(string name)
    {
        var games = await _gameService.GetAllGamesByPublisherNameAsync(name);

        return Ok(games);
    }

    /// <summary>
    /// Asynchronously gets all games by platform type ID.
    /// </summary>
    /// <param name="id">The ID of the platform type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult that encapsulates the result of the action method.</returns>
    [HttpGet("genre/{id}")]
    public async Task<IActionResult> GetAllGamesByPlatformType(int id)
    {
        var games = await _gameService.GetAllGamesByPlatformTypeAsync(id);

        return Ok(games);
    }

    /// <summary>
    /// Asynchronously gets all games by platform type.
    /// </summary>
    /// <param name="type">The type of the platform.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult that encapsulates the result of the action method.</returns>
    [HttpGet("/platform/{type}")]
    public async Task<IActionResult> GetAllGamesByPlatformType(string type)
    {
        var games = await _gameService.GetAllGamesByPlatformTypeAsync(type);

        return Ok(games);
    }

    /// <summary>
    /// Retrieves all comments associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A collection of comments associated with the game.</returns>
    [HttpGet("{gameAlias}/comments")]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetCommentsByGameAlias(string gameAlias)
    {
        var comments = await _gameService.GetCommentsByGameAliasAsync(gameAlias);

        return Ok(comments);
    }

    /// <summary>
    /// Adds a comment to a game specified by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to which the comment will be added.</param>
    /// <param name="commentWrapper">The comment to be added.</param>
    /// <returns>A list of comments for the game including the newly added comment if the operation is successful; otherwise, an error message.</returns>
    [HttpPost("{gameAlias}/comments")]
    public async Task<IActionResult> AddComment(string gameAlias, [FromBody] CommentWrapper commentWrapper)
    {
        await _gameService.AddCommentAsync(commentWrapper, gameAlias);

        var comments = await _gameService.GetCommentsByGameAliasAsync(gameAlias);

        return Ok(comments);
    }

    /// <summary>
    /// Deletes a comment specified by its ID and Game Alias.
    /// </summary>
    /// <returns>A message indicating the result of the operation.</returns>
    [HttpDelete("{gameAlias}/deleteComment/{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId, string gameAlias)
    {
        await _gameService.DeleteComment(commentId);

        var comments = await _gameService.GetCommentsByGameAliasAsync(gameAlias);

        return Ok(comments);
    }

    /// <summary>
    /// Gets the possible durations for a ban.
    /// </summary>
    /// <returns>An IActionResult that represents the result of the action.</returns>
    [HttpGet("getBanDurations")]
    public async Task<IActionResult> GetBanDurations()
    {
        var banDurationsOptions = await _gameService.GetBanDurationsAsync();

        return Ok(banDurationsOptions);
    }

    /// <summary>
    /// Bans a user.
    /// </summary>
    /// <param name="request">The ban request.</param>
    /// <returns>An IActionResult that represents the result of the action.</returns>
    [HttpPost("banUser")]
    public async Task<IActionResult> BanUser([FromBody] BanRequest request)
    {
        await _gameService.BanUserAsync(request.User, request.Duration);
        return Ok();
    }

    /// <summary>
    /// Asynchronously gets the options for the published date filter.
    /// </summary>
    /// <returns>An IActionResult that represents the result of the action.</returns>
    [HttpGet("getPublishedDateOptionsApiUrl")]
    public async Task<IActionResult> GetPublishedDateOption()
    {
        var options = await _gameService.GetPublishedDateOptions();

        return Ok(options);
    }

    /// <summary>
    /// Asynchronously gets the options for the sorting type filter.
    /// </summary>
    /// <returns>An IActionResult that represents the result of the action.</returns>
    [HttpGet("getSortingOptionsApiUrl")]
    public async Task<IActionResult> GetSortingOptionsApiUrl()
    {
        var options = await _gameService.GetSortingOptions();

        return Ok(options);
    }

    /// <summary>
    /// Asynchronously gets the options for the page count filter.
    /// </summary>
    /// <returns>An IActionResult that represents the result of the action.</returns>
    [HttpGet("getPagingOptionsApiUrl")]
    public async Task<IActionResult> GetPagingOptionsApiUrl()
    {
        var options = await _gameService.GetPagingOptions();

        return Ok(options);
    }

    /// <summary>
    /// Generates the content for a game file.
    /// </summary>
    /// <param name="game">The game object to generate the content for.</param>
    /// <returns>A string containing the game alias, name, and description.</returns>
    private static string GenerateGameFileContent(GameResponse? game)
    {
        var content = $"Game Alias: {game.Key}\n" +
                         $"Name: {game.Name}\n" +
                         $"Description: {game.Description}";
        return content;
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