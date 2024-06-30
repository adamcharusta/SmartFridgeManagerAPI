using Microsoft.EntityFrameworkCore;
using SmartFridgeManagerAPI.Application.WeatherForecast.Queries;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.UnitTests.Infrastructure;

namespace SmartFridgeManagerAPI.UnitTests.WeatherForecast.Queries;

public class GetWeatherForecastsQueryTests : UnitTestFactory<GetWeatherForecastsQueryHandler>
{
    [Fact]
    public async Task ShouldReturnListOfWeatherForecasts()
    {
        GetWeatherForecastsQuery request = new();

        IEnumerable<Application.WeatherForecast.Queries.WeatherForecast> result = await _mediator.Send(request);

        result.Count().Should().Be(5);
        _rabbitMq.Received(1).BasicPublish(Arg.Any<EmailQueue>());
    }

    [Fact]
    public async Task DbContextExampleTest()
    {
        List<User> users =
        [
            new User
            {
                Username = "Test",
                Email = "Test",
                Password = "Test",
                ActivationToken = new ActivationToken { Token = "" },
                ResetPasswordTokens = new List<ResetPasswordToken>()
            },
            new User
            {
                Username = "Test2",
                Email = "Test2",
                Password = "Test2",
                ActivationToken = new ActivationToken { Token = "" },
                ResetPasswordTokens = new List<ResetPasswordToken>()
            },
            new User
            {
                Username = "Test3",
                Email = "Test3",
                Password = "Test3",
                ActivationToken = new ActivationToken { Token = "" },
                ResetPasswordTokens = new List<ResetPasswordToken>()
            }
        ];

        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        List<User> result = await _dbContext.Users.ToListAsync();

        result.Count().Should().Be(users.Count());
        _rabbitMq.Received(0).BasicPublish(Arg.Any<EmailQueue>());
    }
}
