using SmartFridgeManagerAPI.Application.WeatherForecasts.Queries;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.UnitTests.Infrastructure;

namespace SmartFridgeManagerAPI.UnitTests.WeatherForecastTests.Queries;

public class GetWeatherForecastsQueryTests : UnitTestFactory<object, GetWeatherForecastsQueryHandler>
{
    [Fact]
    public async Task ShouldReturnListOfWeatherForecasts()
    {
        IEnumerable<WeatherForecast> data = Helper.GetWeatherForecasts();

        _redis.ReadAsync<IEnumerable<WeatherForecast>>(Arg.Any<string>())
            .Returns(data);
        GetWeatherForecastsQuery request = new();

        IEnumerable<WeatherForecast> result = await _mediator.Send(request);

        result.Count().Should().Be(5);
        _rabbitMq.Received(1).BasicPublish(Arg.Any<EmailQueue>());
        await _redis.Received(1).ReadAsync<IEnumerable<WeatherForecast>>(Arg.Any<string>());
    }

    [Fact]
    public async Task ShouldReceivedOnceRedisSaveResult()
    {
        _redis.ReadAsync<IEnumerable<WeatherForecast>>(Arg.Any<string>())
            .ReturnsNull();

        GetWeatherForecastsQuery request = new();

        await _mediator.Send(request);

        await _redis.Received(1)
            .SaveAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<DateTime>());
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
                ActivationTokens = new List<ActivationToken> { new() },
                ResetPasswordTokens = new List<ResetPasswordToken>()
            },
            new User
            {
                Username = "Test2",
                Email = "Test2",
                Password = "Test2",
                ActivationTokens = new List<ActivationToken> { new() },
                ResetPasswordTokens = new List<ResetPasswordToken>()
            },
            new User
            {
                Username = "Test3",
                Email = "Test3",
                Password = "Test3",
                ActivationTokens = new List<ActivationToken> { new() },
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
