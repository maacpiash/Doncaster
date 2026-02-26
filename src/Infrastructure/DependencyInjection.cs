using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        return services;
    }
}
