using SmartFridgeManagerAPI.Application.WeatherForecasts.Queries;
using SmartFridgeManagerAPI.WebAPI.Infrastructure;

namespace SmartFridgeManagerAPI.WebAPI.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetWeatherForecasts);
    }

    private async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender)
    {
        return await sender.Send(new GetWeatherForecastsQuery());
    }
}
