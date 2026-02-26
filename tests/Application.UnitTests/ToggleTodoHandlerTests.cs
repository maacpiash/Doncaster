using FluentAssertions;
using NSubstitute;
using Todo.Application.Features.ToggleTodo;
using Todo.Application.Interfaces;
using Todo.Domain.Exceptions;
using Todo.Domain.Models;

namespace Todo.Application.Tests;

public class ToggleTodoHandlerTests
{
    private readonly ITodoRepository _repository = Substitute.For<ITodoRepository>();
    private readonly ToggleTodoHandler _handler;

    public ToggleTodoHandlerTests()
    {
        _handler = new ToggleTodoHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingTodo_TogglesCompletion()
    {
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem { Id = todoId, Title = "Test", IsCompleted = false };
        _repository.GetByIdAsync(todoId, Arg.Any<CancellationToken>()).Returns(existingTodo);
        _repository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(ci.Arg<TodoItem>()));

        var result = await _handler.Handle(new ToggleTodoCommand(todoId, true), CancellationToken.None);

        result.IsCompleted.Should().BeTrue();
        result.Id.Should().Be(todoId);
        result.Title.Should().Be("Test");
        await _repository.Received(1).UpdateAsync(
            Arg.Is<TodoItem>(t => t.IsCompleted == true && t.Id == todoId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistingTodo_ThrowsTodoNotFoundException()
    {
        var todoId = Guid.NewGuid();
        _repository.GetByIdAsync(todoId, Arg.Any<CancellationToken>()).Returns((TodoItem?)null);

        var act = async () => await _handler.Handle(new ToggleTodoCommand(todoId, true), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<TodoNotFoundException>();
        ex.Which.TodoId.Should().Be(todoId);
    }

    [Fact]
    public async Task Handle_SetToFalse_SetsIsCompletedFalse()
    {
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem { Id = todoId, Title = "Test", IsCompleted = true };
        _repository.GetByIdAsync(todoId, Arg.Any<CancellationToken>()).Returns(existingTodo);
        _repository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(ci.Arg<TodoItem>()));

        var result = await _handler.Handle(new ToggleTodoCommand(todoId, false), CancellationToken.None);

        result.IsCompleted.Should().BeFalse();
    }
}
