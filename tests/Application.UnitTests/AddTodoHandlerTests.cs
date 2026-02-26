using FluentAssertions;
using NSubstitute;
using Todo.Application.Features.AddTodo;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Tests;

public class AddTodoHandlerTests
{
    private readonly ITodoRepository _repository = Substitute.For<ITodoRepository>();
    private readonly AddTodoHandler _handler;

    public AddTodoHandlerTests()
    {
        _handler = new AddTodoHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidTitle_CreatesTodoAndReturnsIt()
    {
        _repository.AddAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(ci.Arg<TodoItem>()));

        var result = await _handler.Handle(new AddTodoCommand("Buy milk"), CancellationToken.None);

        result.Title.Should().Be("Buy milk");
        result.IsCompleted.Should().BeFalse();
        result.Id.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TitleWithWhitespace_TrimsTitle()
    {
        _repository.AddAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(ci.Arg<TodoItem>()));

        var result = await _handler.Handle(new AddTodoCommand("  Buy milk  "), CancellationToken.None);

        result.Title.Should().Be("Buy milk");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_EmptyOrNullTitle_ThrowsArgumentException(string? title)
    {
        var act = async () => await _handler.Handle(new AddTodoCommand(title!), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>());
    }
}
