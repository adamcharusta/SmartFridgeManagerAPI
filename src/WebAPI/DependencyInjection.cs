using SmartFridgeManagerAPI.WebAPI.Infrastructure;

namespace SmartFridgeManagerAPI.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddExceptionHandler<CustomExceptionHandler>();
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
