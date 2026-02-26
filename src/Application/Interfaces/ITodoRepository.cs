using Todo.Domain.Models;

namespace Todo.Application.Interfaces;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(Guid todoId, CancellationToken cancellationToken);
    Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken);
    Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid todoId, CancellationToken cancellationToken);
}
