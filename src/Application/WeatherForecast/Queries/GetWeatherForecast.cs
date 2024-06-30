using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.Infrastructure.Services;

namespace SmartFridgeManagerAPI.Application.WeatherForecast.Queries;

public record GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>;

public class GetWeatherForecastsQueryHandler(IRabbitMqService rabbitMqService)
    : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request,
        CancellationToken cancellationToken)
    {
        Random rng = new();

        IEnumerable<WeatherForecast> result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        });

        rabbitMqService.BasicPublish(new EmailQueue("test@test.com", "Test subject", "test body"));

        return await Task.FromResult(result);
    }
}
