using Mediator;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Features.GetTodos;

public sealed class GetTodosHandler : IRequestHandler<GetTodosQuery, IReadOnlyList<TodoItem>>
{
    private readonly ITodoRepository _repository;

    public GetTodosHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IReadOnlyList<TodoItem>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
