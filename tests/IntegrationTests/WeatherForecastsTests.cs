using System.Net;
using Ardalis.GuardClauses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SmartFridgeManagerAPI.Application.Common.Exceptions;
using SmartFridgeManagerAPI.Application.WeatherForecast.Queries;
using SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

namespace SmartFridgeManagerAPI.IntegrationTests;

public class WeatherForecastsTests(WebApplicationFactory<Program> factory) : IntegrationTestsFactory(factory)
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [Fact]
    public async Task WeatherForecast_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/WeatherForecasts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsNotFound()
    {
        _mediatorFake.Send(Arg.Any<GetWeatherForecastsQuery>()).ThrowsAsync(new NotFoundException("", ""));

        HttpResponseMessage response = await _client.GetAsync("/WeatherForecasts");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsForbidden()
    {
        _mediatorFake.Send(Arg.Any<GetWeatherForecastsQuery>()).ThrowsAsync(new ForbiddenAccessException());

        HttpResponseMessage response = await _client.GetAsync("/WeatherForecasts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
