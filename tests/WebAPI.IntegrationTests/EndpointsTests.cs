using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Domain.Models;

namespace Todo.Presentation.IntegrationTests;

public class EndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_EmptyStore_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        todos.Should().NotBeNull();
    }

    [Fact]
    public async Task PostTodo_ValidTitle_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/", new { Title = "Integration test todo" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Integration test todo");
        todo.IsCompleted.Should().BeFalse();
        todo.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostTodo_EmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/", new { Title = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostAndGetTodo_ReturnsCreatedTodo()
    {
        var createResponse = await _client.PostAsJsonAsync("/", new { Title = "Round trip test" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var getResponse = await _client.GetAsync("/");
        var todos = await getResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

        todos.Should().Contain(t => t.Id == created!.Id && t.Title == "Round trip test");
    }

    [Fact]
    public async Task PatchTodo_ExistingTodo_TogglesCompletion()
    {
        var createResponse = await _client.PostAsJsonAsync("/", new { Title = "Toggle test" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var patchResponse = await _client.PatchAsJsonAsync($"/{created!.Id}", new { IsCompleted = true });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await patchResponse.Content.ReadFromJsonAsync<TodoItem>();
        toggled!.IsCompleted.Should().BeTrue();
        toggled.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task PatchTodo_NonExistingTodo_ReturnsNotFound()
    {
        var response = await _client.PatchAsJsonAsync($"/{Guid.NewGuid()}", new { IsCompleted = true });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_ExistingTodo_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/", new { Title = "Delete test" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var deleteResponse = await _client.DeleteAsync($"/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_NonExistingTodo_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FullCrudLifecycle()
    {
        // Create
        var createResponse = await _client.PostAsJsonAsync("/", new { Title = "Lifecycle test" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        // Toggle
        var patchResponse = await _client.PatchAsJsonAsync($"/{todo!.Id}", new { IsCompleted = true });
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await patchResponse.Content.ReadFromJsonAsync<TodoItem>();
        toggled!.IsCompleted.Should().BeTrue();

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/{todo.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deleted - patch should return 404
        var verifyResponse = await _client.PatchAsJsonAsync($"/{todo.Id}", new { IsCompleted = false });
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
