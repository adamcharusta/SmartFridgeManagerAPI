using SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;
using SmartFridgeManagerAPI.Application.Auth.Dtos;
using SmartFridgeManagerAPI.WebAPI.Infrastructure;

namespace SmartFridgeManagerAPI.WebAPI.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(RegisterUser, "register");
    }

    private async Task<AuthResponse> RegisterUser(ISender sender, RegisterUserCommand command)
    {
        return await sender.Send(command);
    }
}
