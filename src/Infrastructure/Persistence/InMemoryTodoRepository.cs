using System.Collections.Concurrent;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Infrastructure.Persistence;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<TodoItem> items = _items.Values.OrderBy(item => item.Title).ToList();
        return Task.FromResult(items);
    }

    public Task<TodoItem?> GetByIdAsync(Guid todoId, CancellationToken cancellationToken)
    {
        _items.TryGetValue(todoId, out var todo);
        return Task.FromResult(todo);
    }

    public Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        _items[todoItem.Id] = todoItem;
        return Task.FromResult(todoItem);
    }

    public Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        _items[todoItem.Id] = todoItem;
        return Task.FromResult(todoItem);
    }

    public Task<bool> DeleteAsync(Guid todoId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_items.TryRemove(todoId, out _));
    }
}
