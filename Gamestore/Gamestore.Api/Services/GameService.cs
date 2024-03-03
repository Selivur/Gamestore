using System.Globalization;
using Gamestore.Api.Models.DTO.CommentDTO;
using Gamestore.Api.Models.DTO.GameDTO;
using Gamestore.Api.Models.Wrappers.Comment;
using Gamestore.Api.Models.Wrappers.Game;
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
    private readonly IGameRepository _gameRepository;
    private readonly ICommentService _commentService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </summary>
    /// <param name="repository">The game repository providing data access for the service.</param>
    /// <param name="commentService">The comment service providing comment related operations for the service.</param>
    public GameService(IGameRepository repository, ICommentService commentService, IUserService userService)
    {
        _gameRepository = repository;
        _commentService = commentService;
        _userService = userService;
    }

    /// <inheritdoc/>
    public async Task CreateGameAsync(GameWrapper fullGame)
    {
        var game = fullGame.GameRequest;

        game.GameAlias = string.IsNullOrEmpty(game.GameAlias)
            ? NormalizeGameAlias(game.Name)
            : game.GameAlias;

        var existingGame = await _gameRepository.GetByAliasAsync(game.GameAlias);

        if (existingGame != null)
        {
            throw new InvalidOperationException("Game alias must be unique");
        }

        Game newGame = new()
        {
            GameAlias = game.GameAlias,
            Name = game.Name,
            Price = game.Price,
            UnitInStock = game.UnitInStock,
            Discount = game.Discount,
            Description = game.Description,
        };

        await _gameRepository.AddGameWithDependencies(newGame, fullGame.GenresId, fullGame.PlatformsId, fullGame.PublisherId);
    }

    /// <inheritdoc/>
    public async Task<GameResponse?> GetGameByIdAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Game not found");

        return GameResponse.FromGame(game);
    }

    /// <inheritdoc/>
    public async Task<GameResponse?> GetGameResponseByAliasAsync(string gameAlias)
    {
        var game = await _gameRepository.GetByAliasAsync(gameAlias)
            ?? throw new KeyNotFoundException("Game not found");

        return GameResponse.FromGame(game);
    }

    /// <inheritdoc/>
    public async Task<Game> GetGameByAliasAsync(string gameAlias)
    {
        var game = await _gameRepository.GetByAliasAsync(gameAlias)
            ?? throw new KeyNotFoundException("Game not found");

        return game;
    }

    /// <inheritdoc/>
    public async Task UpdateGameAsync(GameWrapper fullGame)
    {
        var game = fullGame.GameRequest;
        game.GameAlias = string.IsNullOrEmpty(game.GameAlias)
            ? NormalizeGameAlias(game.Name)
            : game.GameAlias;

        var existingGame = await _gameRepository.GetByIdAsync(Convert.ToInt32(game.Id))
            ?? throw new KeyNotFoundException("Can't find the Game with this Alias");

        existingGame.GameAlias = game.GameAlias;
        existingGame.Name = game.Name;
        existingGame.Price = game.Price;
        existingGame.UnitInStock = game.UnitInStock;
        existingGame.Discount = game.Discount;
        existingGame.Description = game.Description;

        await _gameRepository.UpdateGameWithDependencies(existingGame, fullGame.GenresId, fullGame.PlatformsId, fullGame.PublisherId);
    }

    /// <inheritdoc/>
    public async Task UpdateGameWithoutDependenciesAsync(Game game)
    {
        game.GameAlias = string.IsNullOrEmpty(game.GameAlias)
            ? NormalizeGameAlias(game.Name)
            : game.GameAlias;

        var existingGame = await _gameRepository.GetByIdAsync(Convert.ToInt32(game.Id))
            ?? throw new KeyNotFoundException("Can't find the Game with this Alias");

        existingGame.GameAlias = game.GameAlias;
        existingGame.Name = game.Name;
        existingGame.Price = game.Price;
        existingGame.UnitInStock = game.UnitInStock;
        existingGame.Discount = game.Discount;
        existingGame.Description = game.Description;

        await _gameRepository.UpdateAsync(existingGame);
    }

    /// <inheritdoc/>
    public async Task RemoveGameAsync(string gameAlias)
    {
        await _gameRepository.RemoveAsync(gameAlias);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync(int[]? genres, int? maxPrice, int? minPrice, string? name, string? datePublishing, string? sort, int? page, int? pageCount, string? trigger)
    {
        var games = await _gameRepository.GetAllAsync();

        games = FilterByGenre(games, genres);
        games = FilterByPrice(games, minPrice, maxPrice);
        games = FilterByName(games, name);
        games = FilterByPublishingDate(games, datePublishing);

        games = SortGames(games, sort);

        if (page.HasValue && pageCount.HasValue)
        {
            games = games.Skip((page.Value - 1) * pageCount.Value).Take(pageCount.Value);
        }

        var gameResponses = games.Select(GameResponse.FromGame).ToList();

        return gameResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResponse>> GetAllGamesByPublisherNameAsync(string name)
    {
        var games = await _gameRepository.GetByPublisherNameAsync(name);

        var gameResponses = games.Select(GameResponse.FromGame).ToList();

        return gameResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResponse>> GetAllGamesByPlatformTypeAsync(int id)
    {
        var games = await _gameRepository.GetByGenreIdAsync(id);

        var gameResponses = games.Select(GameResponse.FromGame).ToList();

        return gameResponses;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResponse>> GetAllGamesByPlatformTypeAsync(string type)
    {
        var games = await _gameRepository.GetByPlatformTypeAsync(type);

        var gameResponses = games.Select(GameResponse.FromGame).ToList();

        return gameResponses;
    }

    /// <inheritdoc/>
    public async Task AddCommentAsync(CommentWrapper commentWrapper, string gameAlias)
    {
        var game = await _gameRepository.GetByAliasAsync(gameAlias)
                   ?? throw new KeyNotFoundException("Game not found");

        await _commentService.AddCommentAsync(commentWrapper, game);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommentResponse>> GetCommentsByGameAliasAsync(string gameAlias)
    {
        var game = await _gameRepository.GetByAliasAsync(gameAlias)
                   ?? throw new KeyNotFoundException("Game not found");

        var comments = await _commentService.GetCommentsByGameIdAsync(game.Id);

        return comments;
    }

    /// <inheritdoc/>
    public async Task DeleteComment(int commentId)
    {
        await _commentService.RemoveCommentAsync(commentId);
    }

    /// <inheritdoc/>
    public Task<string[]> GetBanDurationsAsync()
    {
        return _userService.GetBanDurationsAsync();
    }

    /// <inheritdoc/>
    public async Task<string[]> GetPublishedDateOptions()
    {
        return await Task.FromResult(new string[] { "last week", "last month", "last year", "2 years", "3 years" });
    }

    /// <inheritdoc/>
    public async Task<string[]> GetSortingOptions()
    {
        return await Task.FromResult(new string[] { "Most popular", "Most popular", "Price ASC", "Price DESC", "New" });
    }

    /// <inheritdoc/>
    public async Task<string[]> GetPagingOptions()
    {
        return await Task.FromResult(new string[] { "10", "20", "50", "100", "all" });
    }

    /// <inheritdoc/>
    public Task BanUserAsync(string userName, string banDuration)
    {
        return _userService.BanUserAsync(userName, banDuration);
    }

    /// <inheritdoc/>
    public async Task<GameResponse> GetGameByAliasWithViewsUpdateAsync(string gameAlias)
    {
        var game = GetGameByAliasAsync(gameAlias);

        await UpdateViews(await game);

        return GameResponse.FromGame(await game);
    }

    /// <inheritdoc/>
    public async Task<GameResponse> GetGameByIdWithViewsUpdateAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Game not found");

        await UpdateViews(game);

        return GameResponse.FromGame(game);
    }

    private async Task UpdateViews(Game game)
    {
        game.Views += 1;
        await _gameRepository.UpdateAsync(game);
    }

    private static IEnumerable<Game> FilterByGenre(IEnumerable<Game> games, int[]? genres)
    {
        if (genres is { Length: > 0 })
        {
            games = games.Where(game => game.Genres.Any(genre => genres.Contains(genre.Id)));
        }

        return games;
    }

    private static IEnumerable<Game> FilterByPrice(IEnumerable<Game> games, int? minPrice, int? maxPrice)
    {
        if (maxPrice > 0)
        {
            games = games.Where(game => game.Price <= maxPrice);
        }

        if (minPrice > 0)
        {
            games = games.Where(game => game.Price >= minPrice);
        }

        return games;
    }

    private static IEnumerable<Game> FilterByName(IEnumerable<Game> games, string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            games = games.Where(game => game.Name.Contains(name));
        }

        return games;
    }

    private static IEnumerable<Game> FilterByPublishingDate(IEnumerable<Game> games, string? datePublishing)
    {
        if (!string.IsNullOrEmpty(datePublishing))
        {
            if (DateTime.TryParse(datePublishing, out DateTime date))
            {
                games = games.Where(game => game.PublishDate >= date);
            }
        }

        return games;
    }

    private static IEnumerable<Game> SortGames(IEnumerable<Game> games, string? sort)
    {
        if (string.IsNullOrEmpty(sort))
        {
            return games;
        }

        switch (sort)
        {
            case "Most popular":
                games = games.OrderByDescending(game => game.Views);
                break;
            case "Most commented":
                games = games.OrderByDescending(game => game.Comments.Count);
                break;
            case "Price ASC":
                games = games.OrderBy(game => game.Price);
                break;
            case "Price DESC":
                games = games.OrderByDescending(game => game.Price);
                break;
            case "New":
                games = games.OrderByDescending(game => game.PublishDate);
                break;
            default:
                break;
        }

        return games;
    }

    private static string NormalizeGameAlias(string name)
    {
        return name.Replace(" ", "-").ToLower(CultureInfo.InvariantCulture);
    }
}
