using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly AppDbContext _context;

    public RegisterUserCommandValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(126)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage(
                "Username contains invalid characters. Only letters, numbers, underscores, and hyphens are allowed.")
            .MustAsync(MustUniqueUsername).WithMessage("Username must be unique");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
            .MustAsync(MustUniqueEmail).WithMessage("Email must be unique");

        RuleFor(x => x.ConfirmEmail)
            .NotEmpty()
            .Matches(x => x.Email);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(64).WithMessage("Password must be at most 64 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Matches(x => x.Password);
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
