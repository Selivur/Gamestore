using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities;
using Gamestore.Database.Entities.Enums;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Gamestore.Database.Repositories;

/// <summary>
/// Provides methods for managing comments in the database.
/// Implements the <see cref="ICommentRepository"/> interface.
/// </summary>
public class CommentRepository : ICommentRepository
{
    private readonly GamestoreContext _context;
    private readonly DataBaseLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommentRepository(GamestoreContext context)
    {
        _context = context;
        _logger = new DataBaseLogger();
    }

    /// <inheritdoc />
    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Comment>> GetAllByGameIdAsync(int gameId)
    {
        return await _context.Comments
            .Where(comment => comment.Game.Id == gameId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);

        var clonedComment = (Comment)comment.Clone();
        clonedComment.Game = null;

        await SaveChangesAsync("Error when adding the comment to the database.", CrudOperation.Add, null, clonedComment.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id)
    {
        var comment = await _context.Comments.SingleOrDefaultAsync(g => g.Id.Equals(id))
                   ?? throw new ArgumentException($"No comment found with the ID '{id}'.", nameof(id));

        _context.Comments.Remove(comment);

        var clonedComment = (Comment)comment.Clone();
        clonedComment.Game = null;
        clonedComment.ChildComments = null;

        await SaveChangesAsync("Error when deleting the game from the database.", CrudOperation.Delete, clonedComment.ToBsonDocument(), null);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Comment comment)
    {
        _context.Entry(comment).State = EntityState.Modified;

        var oldObject = await _context.Comments.AsNoTracking().FirstAsync(o => o.Id == comment.Id);
        oldObject.Game = null;
        oldObject.ChildComments = null;

        var clonedComment = (Comment)comment.Clone();
        clonedComment.Game = null;
        clonedComment.ChildComments = null;

        await SaveChangesAsync("Error when updating the comment in the database.", CrudOperation.Update, oldObject.ToBsonDocument(), clonedComment.ToBsonDocument());
    }

    /// <inheritdoc />
    public async Task<Comment?> GetByIdWithChildrenAsync(int id)
    {
        return await _context.Comments
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Asynchronously saves changes to the database context and throws a <see cref="DbUpdateException"/>
    /// with the specified error message if no changes were saved.
    /// </summary>
    /// <param name="errorMessage">The error message to be included in the exception if no changes were saved.</param>
    /// <returns>An asynchronous task representing the operation's completion or throwing a <see cref="DbUpdateException"/>.</returns>
    private async Task SaveChangesAsync(string errorMessage, CrudOperation operation, BsonDocument oldObject, BsonDocument newObject)
    {
        var saved = await _context.SaveChangesAsync();

        if (saved == 0)
        {
            throw new DbUpdateException(errorMessage);
        }

        _logger.LogChange(
            action: operation,
            entityType: typeof(ProductSupplier).FullName,
            oldObject: oldObject,
            newObject: newObject);
    }
}
