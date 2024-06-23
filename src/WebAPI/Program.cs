using SmartFridgeManagerAPI.Application;
using SmartFridgeManagerAPI.Infrastructure;
using SmartFridgeManagerAPI.WebAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string logFilePath = GetLoggerPath(builder);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"{logFilePath}/WebApi/webapi-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    builder.Services
        .AddInfrastructureServices(logFilePath)
        .AddApplicationServices(logFilePath)
        .AddWebAPIServices();

    WebApplication app = builder.Build();

    app
        .UseWebAPI()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while configuring the web api services.");
}
finally
{
    Log.CloseAndFlush();
}

static string GetLoggerPath(WebApplicationBuilder builder)
{
    string? relativeLogFilePath = builder.Configuration.GetValue<string>("LoggerFilePath");
    Guard.Against.NullOrEmpty(relativeLogFilePath, "Please provide a valid path for the log file: 'LoggerFilePath'.");
    string logFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativeLogFilePath));

    return logFilePath;
}

public partial class Program;
