using Gamestore.Api.Models.DTO.GenreDTO;
using Gamestore.Database.Entities;

namespace Gamestore.Api.Services.Interfaces;

/// <summary>
/// Service for managing genres.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Gets all genres.
    /// </summary>
    /// <returns>The collection of all genres.</returns>
    Task<IEnumerable<GenreResponse>> GetAllGenresAsync();

    /// <summary>
    /// Adds a new genre.
    /// </summary>
    /// <param name="genre">The data for the new genre.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddGenreAsync(GenreRequest genre);

    /// <summary>
    /// Gets a genre by its name.
    /// </summary>
    /// <param name="name">The name of the genre.</param>
    /// <returns>The detailed information about the genre.</returns>
    Task<GenreResponse?> GetGenreByNameAsync(string name);

    /// <summary>
    /// Gets a genre by its id.
    /// </summary>
    /// <param name="id">The id of the genre.</param>
    /// <returns>The detailed information about the genre.</returns>
    Task<GenreResponse?> GetGenreByIdAsync(int id);

    /// <summary>
    /// Updates a genre by its identifier.
    /// </summary>
    /// <param name="request">The request object containing the identifier and updated data for the genre.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateGenreAsync(GenreUpdateRequest request);

    /// <summary>
    /// Removes a genre by its id.
    /// </summary>
    /// <param name="id">The id of the genre to be removed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveGenreAsync(int id);

    /// <summary>
    /// Initializes predefined genres in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializePredefinedGenresAsync();

    /// <summary>
    /// Gets a list of genres in the form of <see cref="GenreResponse"/>.
    /// </summary>
    /// <param name="gameAlias">The alias of the game to filter genres.</param>
    /// <returns>An asynchronous task representing the operation, returning a list of <see cref="GenreResponse"/>.</returns>
    Task<IEnumerable<GenreResponse>> GetGenresByGameAliasAsync(string gameAlias);

    /// <summary>
    /// Retrieves a list of genres with the specified parent ID.
    /// </summary>
    /// <param name="parentId">The ID of the parent genre.</param>
    /// <returns>
    /// An asynchronous enumerable collection of genres with the specified parent ID.
    /// </returns>
    Task<IEnumerable<Genre>> GetGenresByParentIdAsync(int parentId);
}