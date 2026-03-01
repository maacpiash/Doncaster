using Microsoft.EntityFrameworkCore;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Infrastructure.Persistence;

public sealed class PostgresTodoRepository(TodoDbContext db) : ITodoRepository
{
    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Todos.OrderBy(t => t.Title).ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(Guid todoId, CancellationToken cancellationToken)
    {
        return await db.Todos.FindAsync([todoId], cancellationToken);
    }

    public async Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        db.Todos.Add(todoItem);
        await db.SaveChangesAsync(cancellationToken);
        return todoItem;
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        var existing = await db.Todos.FindAsync([todoItem.Id], cancellationToken);
        if (existing is not null)
            db.Entry(existing).CurrentValues.SetValues(todoItem);
        await db.SaveChangesAsync(cancellationToken);
        return todoItem;
    }

    public async Task<bool> DeleteAsync(Guid todoId, CancellationToken cancellationToken)
    {
        return await db.Todos.Where(t => t.Id == todoId).ExecuteDeleteAsync(cancellationToken) > 0;
    }
}
