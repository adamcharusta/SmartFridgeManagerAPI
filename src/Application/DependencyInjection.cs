using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SmartFridgeManagerAPI.Application.Common.Behaviours;

namespace SmartFridgeManagerAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string loggerPath)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File($"{loggerPath}/Application/application-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehaviour<,>));
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while configuring the application services.");
        }
        finally
        {
            Log.CloseAndFlush();
        }

        return services;
    }
}
