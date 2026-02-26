using Mediator;
using Todo.Domain.Models;

namespace Todo.Application.Features.AddTodo;

public sealed record AddTodoCommand(string Title) : IRequest<TodoItem>;
