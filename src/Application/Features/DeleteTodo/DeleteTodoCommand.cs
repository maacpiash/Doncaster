using Mediator;

namespace Todo.Application.Features.DeleteTodo;

public sealed record DeleteTodoCommand(Guid TodoId) : IRequest;
