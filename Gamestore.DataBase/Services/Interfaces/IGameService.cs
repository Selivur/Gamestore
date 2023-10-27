﻿using Gamestore.Database.Entities;

namespace Gamestore.Database.Services.Interfaces;

public interface IGameService
{
    /// <summary>
    /// Creates a new game in the database.
    /// </summary>
    /// <param name="game">The game object to be created.</param>
    /// <returns>A tuple indicating whether the creation was successful and an error message if applicable.</returns>
    Task<(bool IsSuccess, string? ErrorMessage)> CreateGameAsync(Game game);

    /// <summary>
    /// Retrieves a game from the database by its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to retrieve.</param>
    /// <returns>A tuple representing the result of the operation, including a boolean indicating success, an error message if applicable, and the retrieved game object.</returns>
    /// <remarks>If the game alias is null or empty, the method will return a tuple with a false success value and an error message. If the game is not found in the database, the method will return a tuple with a false success value and an error message. Otherwise, the method will return a tuple with a true success value and the retrieved game object.</remarks>
    Task<(bool IsSuccess, string? ErrorMessage, Game? Game)> GetGameByAliasAsync(string gameAlias);

    /// <summary>
    /// Updates an existing game in the database.
    /// </summary>
    /// <param name="game">The game object containing the updated properties.</param>
    /// <returns>A tuple indicating whether the update was successful and an error message if applicable.</returns>
    Task<(bool IsSuccess, string? ErrorMessage)> UpdateGameAsync(Game game);

    /// <summary>
    /// Removes a game from the database based on its alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to be removed.</param>
    /// <returns>A tuple indicating whether the removal was successful and an error message if applicable.</returns>
    Task<(bool IsSuccess, string? ErrorMessage)> RemoveGameAsync(string gameAlias);

    /// <summary>
    /// Retrieves all games from the database.
    /// </summary>
    /// <returns>An IEnumerable of Game objects.</returns>
    Task<IEnumerable<Game>> GetAllGamesAsync();
}