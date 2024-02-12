using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Provides methods for managing comments in the database.
/// Implements the <see cref="ICommentRepository"/> interface.
/// </summary>
public class CommentRepository : ICommentRepository
{
    private readonly GamestoreContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommentRepository(GamestoreContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Comment>> GetAllByGameIdAsync(int id)
    {
        return await _context.Comments
            .Where(comment => comment.Game.Id == id && comment.ParentId == null)
            .Include(comment => comment.ChildComments)
                .ThenInclude(childComment => childComment.ChildComments)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await SaveChangesAsync("Error when adding the comment to the database.");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        // TODO cascade remove
        var comment = await _context.Comments.SingleOrDefaultAsync(g => g.Id.Equals(id))
                   ?? throw new ArgumentException($"No comment found with the ID '{id}'.", nameof(id));

        _context.Comments.Remove(comment);

        await SaveChangesAsync("Error when deleting the game from the database.");
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Comment comment)
    {
        _context.Entry(comment).State = EntityState.Modified;

        await SaveChangesAsync("Error when updating the comment in the database.");
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }
    }
}
