namespace Todo.Domain.Exceptions;

public sealed class TodoNotFoundException : Exception
{
    public Guid TodoId { get; }

    public TodoNotFoundException(Guid todoId)
        : base($"Todo item with id '{todoId}' was not found.")
    {
        TodoId = todoId;
    }

    public TodoNotFoundException(Guid todoId, Exception? innerException)
        : base($"Todo item with id '{todoId}' was not found.", innerException)
    {
        TodoId = todoId;
    }
}
