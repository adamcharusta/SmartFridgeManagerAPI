using SmartFridgeManagerAPI.Application.Common.Exceptions;
using SmartFridgeManagerAPI.Application.WeatherForecasts.Queries;
using SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

namespace SmartFridgeManagerAPI.IntegrationTests;

public class WeatherForecastsTests(WebApplicationFactory<Program> factory) : IntegrationTestsFactory(factory)
{
    private const string Url = "/WeatherForecasts";

    [Fact]
    public async Task WeatherForecast_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync(Url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsNotFound()
    {
        _mediatorFake.Send(Arg.Any<GetWeatherForecastsQuery>()).ThrowsAsync(new NotFoundException("", ""));

        HttpResponseMessage response = await _client.GetAsync(Url);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsForbidden()
    {
        _mediatorFake.Send(Arg.Any<GetWeatherForecastsQuery>()).ThrowsAsync(new ForbiddenAccessException());

        HttpResponseMessage response = await _client.GetAsync(Url);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
