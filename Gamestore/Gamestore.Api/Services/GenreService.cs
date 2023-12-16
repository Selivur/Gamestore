using Gamestore.Api.Models.DTO.GenreDTO;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;

namespace Gamestore.Api.Services;

/// <summary>
/// Service for managing genres.
/// </summary>
public class GenreService : IGenreService
{
    private readonly IGenreRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreService"/> class.
    /// </summary>
    /// <param name="genreRepository">The repository for genres.</param>
    public GenreService(IGenreRepository genreRepository)
    {
        _repository = genreRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResponse>> GetAllGenresAsync()
    {
        var genres = await _repository.GetAllAsync();

        var genreResponses = genres.Select(GenreResponse.FromGenre).ToList();

        return genreResponses;
    }

    /// <inheritdoc/>
    public async Task AddGenreAsync(GenreRequest genre)
    {
        var existingGenre = await _repository.GetByNameAsync(genre.Name);

        if (existingGenre != null)
        {
            throw new InvalidOperationException("Genre name must be unique");
        }

        Genre newGenre = new()
        {
            Name = genre.Name,
            ParentId = genre.ParentId,
        };

        await _repository.AddAsync(newGenre);
    }

    /// <inheritdoc/>
    public async Task<GenreResponse?> GetGenreByNameAsync(string name)
    {
        var genre = await _repository.GetByNameAsync(name) ?? throw new KeyNotFoundException("Genre not found");

        return GenreResponse.FromGenre(genre);
    }

    /// <inheritdoc/>
    public async Task<GenreResponse?> GetGenreByIdAsync(int id)
    {
        var genre = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Genre not found");

        return GenreResponse.FromGenre(genre);
    }

    /// <inheritdoc/>
    public async Task UpdateGenreAsync(GenreUpdateRequest request)
    {
        var existingGenre = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Can't find the Genre with this id");

        existingGenre.Name = request.Genre.Name;
        existingGenre.ParentId = request.Genre.ParentId;

        await _repository.UpdateAsync(existingGenre);
    }

    /// <inheritdoc/>
    public async Task RemoveGenreAsync(int id)
    {
        await _repository.RemoveAsync(id);
    }

    /// <inheritdoc/>
    public async Task InitializePredefinedGenresAsync()
    {
        var genres = new List<Genre>
        {
            new() { Name = "Strategy" },
            new() { Name = "RTS", ParentId = 1 },
            new() { Name = "TBS", ParentId = 1 },
            new() { Name = "RPG" },
            new() { Name = "Sports" },
            new() { Name = "Races" },
            new() { Name = "Rally", ParentId = 6 },
            new() { Name = "Arcade", ParentId = 6 },
            new() { Name = "Formula", ParentId = 6 },
            new() { Name = "Off-road", ParentId = 6 },
            new() { Name = "Action" },
            new() { Name = "FPS", ParentId = 11 },
            new() { Name = "TPS", ParentId = 11 },
            new() { Name = "Adventure" },
            new() { Name = "Puzzle & Skill" },
            new() { Name = "Misc." },
        };

        foreach (var genre in genres)
        {
            await _repository.AddAsync(genre);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResponse>> GetGenresByGameAliasAsync(string gameAlias)
    {
        var genres = await _repository.GetByGameAliasAsync(gameAlias);

        var genreResponses = genres.Select(GenreResponse.FromGenre).ToList();

        return genreResponses;
    }
}