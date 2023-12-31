﻿using System.Text;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.Api.Controllers;

/// <summary>
/// API controller for managing game-related operations.
/// </summary>
[Route("api/games")]
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
    /// Creates a new game in the database.
    /// </summary>
    /// <param name="game">The game object to be created.</param>
    /// <returns>An IActionResult indicating whether the creation was successful.</returns>
    [HttpPost("new")]
    public async Task<IActionResult> CreateGame([FromBody] GameRequest game)
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
    [HttpGet("{gameAlias}")]
    public async Task<IActionResult> GetGame(string gameAlias)
    {
        var result = await _gameService.GetGameByAliasAsync(gameAlias);

        return Ok(result);
    }

    /// <summary>
    /// Updates a game in the game store.
    /// </summary>
    /// <param name="game">The game object to update.</param>
    /// <returns>An IActionResult object representing the result of the update operation.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateGame([FromBody] GameRequest game)
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
    [HttpDelete("{gameAlias}")]
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
        var game = await _gameService.GetGameByAliasAsync(gameAlias);

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