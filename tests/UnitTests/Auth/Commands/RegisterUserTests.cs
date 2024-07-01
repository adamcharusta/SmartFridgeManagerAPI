using SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;
using SmartFridgeManagerAPI.Application.Auth.Dtos;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.UnitTests.Infrastructure;

namespace SmartFridgeManagerAPI.UnitTests.Auth.Commands;

public class RegisterUserTests : UnitTestFactory<RegisterUserCommand, RegisterUserCommandHandler>
{
    [Fact]
    public async Task RegisterUser_OnCorrectData_ShouldAddNewUserToDb()
    {
        RegisterUserCommand newUser = new()
        {
            Username = "test",
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX",
            ConfirmEmail = "test@test.com",
            Email = "test@test.com",
            ActivationTokenRedirectUrl = "url"
        };

        await _mediator.Send(newUser);

        List<User> result = await _dbContext.Users.Include(x => x.ActivationTokens).ToListAsync();
        User userFromDb = result[0];
        PasswordVerificationResult passwordResult =
            _passwordHasher.VerifyHashedPassword(userFromDb, userFromDb.Password, newUser.Password);

        userFromDb.Username.Should().Be(newUser.Username);
        userFromDb.Email.Should().Be(newUser.Email);
        userFromDb.ActivationTokens!.Count.Should().Be(1);
        result.Count.Should().Be(1);
        _authEmailService.Received(1).SendActivationEmail(userFromDb.Email, newUser.ActivationTokenRedirectUrl,
            userFromDb.ActivationTokens.First().Token);
        passwordResult.Should().Be(PasswordVerificationResult.Success);
    }

    [Theory]
    [InlineData("badUsername,.", "sameEmail@test.com", "sameEmail@test.com", "goodPassword123!", "goodPassword123!",
        "")]
    [InlineData("badUsername,.", "sameEmail@test.com", "sameEmail@test.com", "goodPassword123!", "goodPassword123!",
        "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "notSameEmail@test.com", "goodPassword123!", "goodPassword123!",
        "url")]
    [InlineData("goodUsername", "badEmailFormat", "email@test.com", "goodPassword123!", "goodPassword123!", "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "goodPassword123!", "notSamePassword123!",
        "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "withoutbigletter1!", "goodPassword123!",
        "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "withoutNumber!", "goodPassword123!",
        "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "withoutSymbol1", "goodPassword123!",
        "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "t0Shor!", "t0Shor!", "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com",
        "toL0ng!123123zsdddddddfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdf",
        "toL0ng!123123zsdddddddfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdfdf", "url")]
    [InlineData("", "sameEmail@test.com", "sameEmail@test.com", "goodPassword123!", "goodPassword123!", "url")]
    [InlineData("goodUsername", "", "sameEmail@test.com", "goodPassword123!", "goodPassword123!", "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "", "goodPassword123!", "goodPassword123!", "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "", "goodPassword123!", "url")]
    [InlineData("goodUsername", "sameEmail@test.com", "sameEmail@test.com", "goodPassword123!", "", "url")]
    public async Task RegisterUser_OnIncorrectData_ShouldThrowValidationException(string username, string email,
        string confirmEmail, string password, string confirmPassword, string url)
    {
        RegisterUserCommand newUser = new()
        {
            Username = username,
            Password = email,
            ConfirmPassword = confirmEmail,
            ConfirmEmail = password,
            Email = confirmPassword,
            ActivationTokenRedirectUrl = url
        };

        Func<Task<AuthResponse>> act = async () => await _mediator.Send(newUser);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("Test", "newEmail@test.com")]
    [InlineData("newUsername", "test@test.com")]
    public async Task RegisterUser_OnSameAuthData_ShouldThrowValidationException(string username, string email)
    {
        RegisterUserCommand sameUser = new()
        {
            Username = "Test",
            Email = "test@test.com",
            ConfirmEmail = "test@test.com",
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX",
            ActivationTokenRedirectUrl = "url"
        };

        await _mediator.Send(sameUser);

        RegisterUserCommand newUser = new()
        {
            Username = username,
            Email = email,
            ConfirmEmail = email,
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX",
            ActivationTokenRedirectUrl = "url"
        };

        Func<Task<AuthResponse>> act = async () => await _mediator.Send(newUser);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
