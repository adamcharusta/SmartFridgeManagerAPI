using SmartFridgeManagerAPI.Application.Auth.Services;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Domain.Entities.Events;
using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<int>
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string ConfirmEmail { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RegisterUserCommand, User>()
                .ForMember(d => d.ActivationToken, opt => opt.MapFrom(src => new ActivationToken()))
                .ForMember(d => d.ResetPasswordTokens, opt => opt.MapFrom(src => new List<ResetPasswordToken>()));
        }
    }
}

public class RegisterUserCommandHandler(
    AppDbContext context,
    IPasswordHasher<User> passwordHasher,
    IAuthEmailService authEmailService,
    IMapper mapper)
    : IRequestHandler<RegisterUserCommand, int>
{
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User user = Guard.Against.Null(mapper.Map<User>(request));

        user.Password = passwordHasher.HashPassword(user, request.Password);

        user.AddDomainEvent(new RegisterUserEvent(user));

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        authEmailService.SendActivationEmail(user.Email, user.ActivationToken!.Token);

        return user.Id;
    }
}
