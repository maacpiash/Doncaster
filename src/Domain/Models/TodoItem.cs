namespace Todo.Domain.Models;

public sealed record TodoItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
}
