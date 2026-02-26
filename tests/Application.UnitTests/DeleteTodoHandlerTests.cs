using FluentAssertions;
using NSubstitute;
using Todo.Application.Features.DeleteTodo;
using Todo.Application.Interfaces;
using Todo.Domain.Exceptions;
using Todo.Domain.Models;

namespace Todo.Application.Tests;

public class DeleteTodoHandlerTests
{
    private readonly ITodoRepository _repository = Substitute.For<ITodoRepository>();
    private readonly DeleteTodoHandler _handler;

    public DeleteTodoHandlerTests()
    {
        _handler = new DeleteTodoHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingTodo_DeletesSuccessfully()
    {
        var todoId = Guid.NewGuid();
        _repository.DeleteAsync(todoId, Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await _handler.Handle(new DeleteTodoCommand(todoId), CancellationToken.None);

        await act.Should().NotThrowAsync();
        await _repository.Received(1).DeleteAsync(todoId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistingTodo_ThrowsTodoNotFoundException()
    {
        var todoId = Guid.NewGuid();
        _repository.DeleteAsync(todoId, Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _handler.Handle(new DeleteTodoCommand(todoId), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<TodoNotFoundException>();
        ex.Which.TodoId.Should().Be(todoId);
    }
}
