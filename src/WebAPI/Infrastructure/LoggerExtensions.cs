namespace SmartFridgeManagerAPI.WebAPI.Infrastructure;

public static class LoggerExtensions
{
    public static string GetLoggerFilePath(this WebApplicationBuilder builder)
    {
        string? relativeLogFilePath = builder.Configuration.GetValue<string>("LoggerFilePath");
        Guard.Against.NullOrEmpty(relativeLogFilePath,
            "Please provide a valid path for the log file: 'LoggerFilePath'.");
        string loggerFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativeLogFilePath));

        return loggerFilePath;
    }
}
