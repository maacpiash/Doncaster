using Mediator;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Features.AddTodo;

public sealed class AddTodoHandler : IRequestHandler<AddTodoCommand, TodoItem>
{
    private readonly ITodoRepository _repository;

    public AddTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required.", nameof(request.Title));
        }

        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            IsCompleted = false
        };

        return await _repository.AddAsync(todoItem, cancellationToken);
    }
}
