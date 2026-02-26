using Mediator;
using Microsoft.AspNetCore.Diagnostics;
using Todo.Application.Features.AddTodo;
using Todo.Application.Features.GetTodos;
using Todo.Application.Features.ToggleTodo;
using Todo.Application.Features.DeleteTodo;
using Todo.Domain.Exceptions;
using Todo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddMediator();
builder.Services.AddInfrastructure();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is TodoNotFoundException notFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                message = notFoundException.Message,
                todoId = notFoundException.TodoId
            });
            return;
        }

        if (exception is ArgumentException argumentException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = argumentException.Message,
                parameter = argumentException.ParamName
            });
            return;
        }
    });
});

app.MapGet("/", async (IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("Retrieving all todos");
    var todos = await mediator.Send(new GetTodosQuery(), cancellationToken);
    logger.LogInformation("Retrieved {TodoCount} todos", todos.Count);
    return Results.Ok(todos);
});

app.MapPost("/", async (AddTodoRequest request, IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("Adding todo with title {Title}", request.Title);
    var todo = await mediator.Send(new AddTodoCommand(request.Title), cancellationToken);
    logger.LogInformation("Added todo {TodoId} with title {Title}", todo.Id, todo.Title);
    return Results.Created($"/{todo.Id}", todo);
});

app.MapDelete("/{todoId:guid}", async (Guid todoId, IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("Deleting todo {TodoId}", todoId);
    await mediator.Send(new DeleteTodoCommand(todoId), cancellationToken);
    logger.LogInformation("Deleted todo {TodoId}", todoId);
    return Results.NoContent();
});

app.MapPatch("/{todoId:guid}", async (Guid todoId, ToggleRequest request, IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("Toggling todo {TodoId} to IsCompleted={IsCompleted}", todoId, request.IsCompleted);
    var todo = await mediator.Send(new ToggleTodoCommand(todoId, request.IsCompleted), cancellationToken);
    logger.LogInformation("Toggled todo {TodoId}, IsCompleted={IsCompleted}", todo.Id, todo.IsCompleted);
    return Results.Ok(todo);
});

app.MapDefaultEndpoints();

app.Run();

public sealed record AddTodoRequest(string Title);

public sealed record ToggleRequest(bool IsCompleted);
