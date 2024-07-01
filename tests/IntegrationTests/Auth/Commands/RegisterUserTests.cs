using SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;
using SmartFridgeManagerAPI.Application.Auth.Dtos;
using SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

namespace SmartFridgeManagerAPI.IntegrationTests.Auth.Commands;

public class RegisterUserTests(WebApplicationFactory<Program> factory) : IntegrationTestsFactory(factory)
{
    private const string Url = "/Auth/register";

    [Fact]
    public async Task RegisterUser_OnCorrectData_ShouldReturnOk()
    {
        RegisterUserCommand newUser = new()
        {
            Username = "test",
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX",
            ConfirmEmail = "test@test.com",
            Email = "test@test.com",
            ActivationTokenRedirectUrl = ""
        };

        _mediatorFake.Send(Arg.Any<RegisterUserCommand>()).Returns(new AuthResponse { Message = "" });

        HttpResponseMessage response = await _client.PostAsync(Url, newUser.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_OnIncorrectData_ShouldReturnBadRequest()
    {
        RegisterUserCommand newUser = new()
        {
            Username = "test",
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX",
            ConfirmEmail = "test@test.com",
            Email = "test@test.com",
            ActivationTokenRedirectUrl = ""
        };

        _mediatorFake.Send(Arg.Any<RegisterUserCommand>()).Throws(new ValidationException([]));

        HttpResponseMessage response = await _client.PostAsync(Url, newUser.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
