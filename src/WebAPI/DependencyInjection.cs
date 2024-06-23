using SmartFridgeManagerAPI.WebAPI.Infrastructure;

namespace SmartFridgeManagerAPI.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static WebApplication UseWebAPI(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseExceptionHandler(options => { });
        app.MapEndpoints();

        return app;
    }
}
