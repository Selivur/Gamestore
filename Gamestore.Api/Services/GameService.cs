using System.Globalization;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <inheritdoc/>
public class GameService : IGameService
{
    private readonly IGameRepository _repository;

    public GameService(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> CreateGameAsync(GameRequest game)
    {
        ArgumentNullException.ThrowIfNull(game);
        try
        {
            game.GameAlias ??= NormalizeGameAlias(game.Name);
            var existingGame = await _repository.GetByAliasAsync(game.GameAlias);
            if (existingGame != null)
            {
                return (false, "Game alias must be unique");
            }

            Game newGame = new()
            {
                GameAlias = game.GameAlias,
                Name = game.Name,
                Description = game.Description,
            };
            await _repository.AddAsync(newGame);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<(bool IsSuccess, string? ErrorMessage, GameResponse? Game)> GetGameByAliasAsync(string gameAlias)
    {
        ArgumentNullException.ThrowIfNull(gameAlias);
        try
        {
            var game = await _repository.GetByAliasAsync(gameAlias);
            if (game == null)
            {
                return (false, "Game not found", null);
            }

            GameResponse newGame = new()
            {
                GameAlias = game.GameAlias,
                Name = game.Name,
                Description = game.Description,
            };
            return (true, null, newGame);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateGameAsync(GameRequest game)
    {
        ArgumentNullException.ThrowIfNull(game);
        try
        {
            game.GameAlias ??= NormalizeGameAlias(game.Name);
            Game existingGame = await _repository.GetByAliasAsync(game.GameAlias);
            existingGame.Name = game.Name;
            existingGame.Description = game.Description;
            await _repository.UpdateAsync(existingGame);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> RemoveGameAsync(string gameAlias)
    {
        ArgumentNullException.ThrowIfNull(gameAlias);
        try
        {
            await _repository.RemoveAsync(gameAlias);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
    {
        var games = await _repository.GetAllAsync();
        List<GameResponse> gameResponses = games.Select(game => new GameResponse
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
