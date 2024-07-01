using Microsoft.Extensions.Localization;
using SmartFridgeManagerAPI.Application.Auth.Dtos;
using SmartFridgeManagerAPI.Application.Auth.Services;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Domain.Entities.Events;
using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<AuthResponse>
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string ConfirmEmail { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public required string ActivationTokenRedirectUrl { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RegisterUserCommand, User>()
                .ForMember(d => d.ActivationTokens, opt => opt.MapFrom(src => new List<ActivationToken> { new() }))
                .ForMember(d => d.ResetPasswordTokens, opt => opt.MapFrom(src => new List<ResetPasswordToken>()));
        }
    }
}

public class RegisterUserCommandHandler(
    AppDbContext context,
    IPasswordHasher<User> passwordHasher,
    IAuthEmailService authEmailService,
    IMapper mapper,
    IStringLocalizer<Messages> messages)
    : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User user = Guard.Against.Null(mapper.Map<User>(request));

        user.Password = passwordHasher.HashPassword(user, request.Password);

        user.AddDomainEvent(new RegisterUserEvent(user));

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        authEmailService.SendActivationEmail(user.Email, request.ActivationTokenRedirectUrl,
            user.ActivationTokens!.First().Token);

        return new AuthResponse { Message = messages["RegisteredUserActivationEmailMsg"] };
    }
}
