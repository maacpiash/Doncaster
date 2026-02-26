using Mediator;
using Todo.Application.Interfaces;
using Todo.Domain.Exceptions;
using Todo.Domain.Models;

namespace Todo.Application.Features.ToggleTodo;

public sealed class ToggleTodoHandler : IRequestHandler<ToggleTodoCommand, TodoItem>
{
    private readonly ITodoRepository _repository;

    public ToggleTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<TodoItem> Handle(ToggleTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.TodoId, cancellationToken);
        if (todo is null)
        {
            throw new TodoNotFoundException(request.TodoId);
        }

        var updatedTodo = todo with { IsCompleted = request.IsCompleted };
        return await _repository.UpdateAsync(updatedTodo, cancellationToken);
    }
}
