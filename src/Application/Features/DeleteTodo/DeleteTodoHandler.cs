using Mediator;
using Todo.Application.Interfaces;
using Todo.Domain.Exceptions;

namespace Todo.Application.Features.DeleteTodo;

public sealed class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository _repository;

    public DeleteTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.TodoId, cancellationToken);
        if (!deleted)
        {
            throw new TodoNotFoundException(request.TodoId);
        }

        return Unit.Value;
    }
}
