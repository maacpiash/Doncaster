using FluentAssertions;
using NSubstitute;
using Todo.Application.Features.GetTodos;
using Todo.Application.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Tests;

public class GetTodosHandlerTests
{
    private readonly ITodoRepository _repository = Substitute.For<ITodoRepository>();
    private readonly GetTodosHandler _handler;

    public GetTodosHandlerTests()
    {
        _handler = new GetTodosHandler(_repository);
    }

    [Fact]
    public async Task Handle_ReturnsAllTodos()
    {
        var todos = new List<TodoItem>
        {
            new() { Id = Guid.NewGuid(), Title = "First", IsCompleted = false },
            new() { Id = Guid.NewGuid(), Title = "Second", IsCompleted = true }
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(todos);

        var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        await _repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<TodoItem>());

        var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
