using Microsoft.Extensions.DependencyInjection;

namespace SmartFridgeManagerAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string loggerPath)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File($"{loggerPath}/Infrastructure/infrastructure-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while configuring the web api services.");
        }
        finally
        {
            Log.CloseAndFlush();
        }

        return services;
    }
}
