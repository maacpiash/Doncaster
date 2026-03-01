using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TodoDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<TodoDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<ITodoRepository, PostgresTodoRepository>();
        }
        else
        {
            services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        }

        return services;
    }
}
