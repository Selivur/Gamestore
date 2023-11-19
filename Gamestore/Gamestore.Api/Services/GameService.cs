﻿using System.Globalization;
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

    public async Task<GameResponse?> GetGameByAliasAsync(string gameAlias)
    {
        var game = await _repository.GetByAliasAsync(gameAlias) ?? throw new InvalidOperationException("Game not found");
        GameResponse newGame = new()
        {
            GameAlias = game.GameAlias,
            Name = game.Name,
            Description = game.Description,
        };
        return newGame;
    }

    public async Task UpdateGameAsync(GameRequest game)
    {
        game.GameAlias ??= NormalizeGameAlias(game.Name);
        Game existingGame = await _repository.GetByAliasAsync(game.GameAlias) ?? throw new InvalidOperationException("Can't find the Game with this Alias");
        existingGame.Name = game.Name;
        existingGame.Description = game.Description;
        await _repository.UpdateAsync(existingGame);
    }

    public async Task RemoveGameAsync(string gameAlias)
    {
        await _repository.RemoveAsync(gameAlias);
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
