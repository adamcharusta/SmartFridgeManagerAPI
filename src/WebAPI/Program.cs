using SmartFridgeManagerAPI.Application;
using SmartFridgeManagerAPI.Infrastructure;
using SmartFridgeManagerAPI.WebAPI;
using SmartFridgeManagerAPI.WebAPI.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"{builder.GetLoggerFilePath()}/logs-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    builder.Services
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices()
        .AddWebAPIServices();

    WebApplication app = builder.Build();

    await app
        .UseInfrastructureAsync();

    app
        .UseWebAPI()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while app is running.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
