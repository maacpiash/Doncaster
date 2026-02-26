using Mediator;
using Todo.Domain.Models;

namespace Todo.Application.Features.GetTodos;

public sealed record GetTodosQuery : IRequest<IReadOnlyList<TodoItem>>;
