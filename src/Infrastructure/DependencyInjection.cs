using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartFridgeManagerAPI.Infrastructure.Common.Services;
using SmartFridgeManagerAPI.Infrastructure.Common.Settings;
using SmartFridgeManagerAPI.Infrastructure.Data;
using SmartFridgeManagerAPI.Infrastructure.Data.Interceptors;

namespace SmartFridgeManagerAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddAppDbContext(configuration)
            .AddRabbitMq(configuration)
            .AddRedis(configuration);
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string hostName = Guard.Against.NullOrEmpty(configuration["DB_HOSTNAME"], "DB_HOSTNAME");
        string port = Guard.Against.NullOrEmpty(configuration["DB_PORT"], "DB_PORT");
        string name = Guard.Against.NullOrEmpty(configuration["DB_NAME"], "DB_NAME");
        string user = Guard.Against.NullOrEmpty(configuration["DB_USER"], "DB_USER");
        string password = Guard.Against.NullOrEmpty(configuration["DB_PASS"], "DB_PASS");
        string options = Guard.Against.NullOrEmpty(configuration["DB_OPTIONS"], "DB_OPTIONS");
        string connectionString =
            $"Server={hostName},{port};Database={name};User Id={user};Password={password};{options}";

        services.AddScoped<ISaveChangesInterceptor, BaseEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            opt.UseSqlServer(connectionString);
        });

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddTransient<AppDbContextInitializer>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        string hostname = Guard.Against.NullOrEmpty(configuration["RABBITMQ_HOSTNAME"], "RABBITMQ_HOSTNAME");
        string port = Guard.Against.NullOrEmpty(configuration["RABBITMQ_PORT"], "RABBITMQ_PORT");
        string user = Guard.Against.NullOrEmpty(configuration["RABBITMQ_USER"], "RABBITMQ_USER");
        string password = Guard.Against.NullOrEmpty(configuration["RABBITMQ_PASS"], "RABBITMQ_PASS");

        RabbitMqSettings rabbitMqSettings = new()
        {
            HostName = hostname, Port = Convert.ToInt32(port), UserName = user, Password = password
        };

        services.AddSingleton(rabbitMqSettings);
        services.AddScoped<IRabbitMqService, RabbitMqService>();

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        string hostname = Guard.Against.NullOrEmpty(configuration["REDIS_HOSTNAME"], "REDIS_HOSTNAME");
        string port = Guard.Against.NullOrEmpty(configuration["REDIS_PORT"], "REDIS_PORT");

        RedisSettings redisSettings = new() { HostName = hostname, Port = Convert.ToInt32(port) };

        services.AddSingleton(redisSettings);
        services.AddScoped<IRedisService, RedisService>();

        return services;
    }

    public static async Task<WebApplication> UseInfrastructureAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        AppDbContextInitializer appDbContextInitializer =
            scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        await appDbContextInitializer.InitialiseAsync();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

        app.UseHealthChecks("/health");

        return app;
    }
}
