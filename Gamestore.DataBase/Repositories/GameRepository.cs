using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <inheritdoc />
public class GameRepository : IGameRepository
{
    private readonly GamestoreContext _context;

    public GameRepository(GamestoreContext context)
    {
        _context = context;
    }

    public async Task<Game> GetByIdAsync(int id)
    {
        return await _context.Games.FindAsync(id);
    }

    public async Task<Game> GetByAliasAsync(string gameAlias)
    {
        return await _context.Games.SingleAsync(g => g.GameAlias == gameAlias);
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    public async Task AddAsync(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        _context.Entry(game).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(string gameAlias)
    {
        var game = await _context.Games.SingleAsync(g => g.GameAlias == gameAlias);
        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }
}
