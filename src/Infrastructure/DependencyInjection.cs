using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartFridgeManagerAPI.Infrastructure.Interceptors;

namespace SmartFridgeManagerAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration["DB_CONNECTION_STRING"] ??
                                   configuration.GetConnectionString("DefaultConnection");

        Guard.Against.NullOrEmpty(connectionString,
            "Please provide a valid connection string: 'DB_CONNECTION_STRING'.");

        services.AddScoped<ISaveChangesInterceptor, BaseEntityInterceptor>();

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
