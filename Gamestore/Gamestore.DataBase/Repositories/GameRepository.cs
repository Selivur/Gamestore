﻿using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Implementation of the <see cref="IGameRepository"/> interface.
/// Provides data access functionality for managing game entities.
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for interacting with the underlying data store.</param>
    public GameRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _context.Games.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Game?> GetByAliasAsync(string gameAlias)
    {
        return await _context.Games.SingleOrDefaultAsync(g => g.GameAlias == gameAlias);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Game game)
    {
        _context.Games.Add(game);
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when adding a game to the database.");
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Game game)
    {
        _context.Entry(game).State = EntityState.Modified;
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when updating a game in the database.");
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string gameAlias)
    {
        var game = await _context.Games.SingleAsync(g => g.GameAlias == gameAlias);
        _context.Games.Remove(game);
        var saved = await _context.SaveChangesAsync();
        if (saved == 0)
        {
            throw new InvalidOperationException("Error when deleting a game from the database.");
        }
    }
}
