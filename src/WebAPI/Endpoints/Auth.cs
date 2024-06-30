using SmartFridgeManagerAPI.Application.Auth.Commands.RegisterUser;
using SmartFridgeManagerAPI.WebAPI.Infrastructure;

namespace SmartFridgeManagerAPI.WebAPI.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(RegisterUser, "register");
    }

    private async Task<IResult> RegisterUser(ISender sender, RegisterUserCommand command)
    {
        await sender.Send(command);
        return Results.Created();
    }
}
