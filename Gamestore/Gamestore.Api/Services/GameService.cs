using System.Globalization;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Implementation of the <see cref="IGameService"/> interface.
/// Provides functionality for managing game-related operations.
/// </summary>
public class GameService : IGameService
{
    private readonly IGameRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </summary>
    /// <param name="repository">The game repository providing data access for the service.</param>
    public GameService(IGameRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task CreateGameAsync(GameRequest game)
    {
        game.GameAlias ??= NormalizeGameAlias(game.Name);

        var existingGame = await _repository.GetByAliasAsync(game.GameAlias);

        if (existingGame != null)
        {
            throw new InvalidOperationException("Game alias must be unique");
        }

        Game newGame = new()
        {
            GameAlias = game.GameAlias,
            Name = game.Name,
            Description = game.Description,
        };

        await _repository.AddAsync(newGame);
    }

    /// <inheritdoc/>
    public async Task<GameResponse?> GetGameByAliasAsync(string gameAlias)
    {
        var game = await _repository.GetByAliasAsync(gameAlias) ?? throw new KeyNotFoundException("Game not found");

        GameResponse newGame = new()
        {
            GameAlias = game.GameAlias,
            Name = game.Name,
            Description = game.Description,
        };

        return newGame;
    }

    /// <inheritdoc/>
    public async Task UpdateGameAsync(GameRequest game)
    {
        game.GameAlias ??= NormalizeGameAlias(game.Name);

        var existingGame = await _repository.GetByAliasAsync(game.GameAlias) ?? throw new KeyNotFoundException("Can't find the Game with this Alias");
        existingGame.Name = game.Name;
        existingGame.Description = game.Description;

        await _repository.UpdateAsync(existingGame);
    }

    /// <inheritdoc/>
    public async Task RemoveGameAsync(string gameAlias)
    {
        await _repository.RemoveAsync(gameAlias);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
    {
        var games = await _repository.GetAllAsync();

        var gameResponses = games.Select(game => new GameResponse
        {
            GameAlias = game.GameAlias,
            Name = game.Name,
            Description = game.Description,
        }).ToList();

        return gameResponses;
    }

    private static string NormalizeGameAlias(string name)
    {
        return name.Replace(" ", "-").ToLower(CultureInfo.InvariantCulture);
    }
}
