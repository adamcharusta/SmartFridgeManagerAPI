using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.Infrastructure.Common.Services;

namespace SmartFridgeManagerAPI.Application.WeatherForecasts.Queries;

public record GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>;

public class GetWeatherForecastsQueryHandler(IRabbitMqService rabbitMqService, IRedisService redisService)
    : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request,
        CancellationToken cancellationToken)
    {
        string key = "WeatherForecasts";
        IEnumerable<WeatherForecast>? redisResult =
            await redisService.ReadAsync<IEnumerable<WeatherForecast>>(key);

        rabbitMqService.BasicPublish(new EmailQueue("test@test.com", "Test subject", "test body"));

        if (redisResult == null)
        {
            IEnumerable<WeatherForecast> result = await GetWeatherForecastsAsync();
            await redisService.SaveAsync(key, result, DateTime.Now.AddMinutes(2));
            return result;
        }

        return redisResult;
    }

    private async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
    {
        IEnumerable<WeatherForecast> result = Helper.GetWeatherForecasts();
        await Task.Delay(3000);
        return result;
    }
}

public static class Helper
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


    public static IEnumerable<WeatherForecast> GetWeatherForecasts()
    {
        Random rng = new();

        IEnumerable<WeatherForecast> result = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });

        return result;
    }
}
