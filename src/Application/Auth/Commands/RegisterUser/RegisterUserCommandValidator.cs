using Microsoft.Extensions.Localization;
using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly AppDbContext _context;

    public RegisterUserCommandValidator(AppDbContext context, IStringLocalizer<Properties> properties,
        IStringLocalizer<Messages> messages)
    {
        _context = context;

        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithName(properties["Username"])
            .MaximumLength(126).WithName(properties["Username"])
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithName(properties["Username"])
            .WithMessage(messages["ContainInvalidCharacters"])
            .MustAsync(MustUniqueUsername).WithName(properties["Username"])
            .WithMessage(messages["MustBeUnique"]);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress().WithName(properties["Email"])
            .MustAsync(MustUniqueEmail).WithMessage(messages["MustBeUnique"]);

        RuleFor(x => x.ConfirmEmail)
            .NotEmpty().WithName(properties["ConfirmEmail"])
            .Equal(x => x.Email).WithMessage(string.Format(messages["MustHasSameValue"], properties["ConfirmEmail"],
                properties["Email"]));

        RuleFor(x => x.Password)
            .NotEmpty().WithName(properties["Password"])
            .MinimumLength(8).WithName(properties["Password"])
            .MaximumLength(64).WithName(properties["Password"])
            .Matches("[A-Z]").WithName(properties["Password"]).WithMessage(messages["MustHasOneUppercase"])
            .Matches("[a-z]").WithName(properties["Password"]).WithMessage(messages["MustHasOneLowercase"])
            .Matches("[0-9]").WithName(properties["Password"]).WithMessage(messages["MustHasOneDigit"])
            .Matches("[^a-zA-Z0-9]").WithName(properties["Password"])
            .WithMessage(messages["MustHasOneSpecialCharacter"]);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithName(properties["ConfirmPassword"])
            .Matches(x => x.Password).WithMessage(string.Format(messages["MustHasSameValue"],
                properties["ConfirmPassword"],
                properties["Password"]));

        RuleFor(x => x.ActivationTokenRedirectUrl)
            .MaximumLength(256)
            .NotEmpty();
    }

    private async Task<bool> MustUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AllAsync(u => u.Username.ToLower() != username.ToLower(), cancellationToken);
    }

    private async Task<bool> MustUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AllAsync(u => u.Email.ToLower() != email.ToLower(), cancellationToken);
    }
}
