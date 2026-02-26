using Mediator;
using Todo.Domain.Models;

namespace Todo.Application.Features.ToggleTodo;

public sealed record ToggleTodoCommand(Guid TodoId, bool IsCompleted) : IRequest<TodoItem>;
