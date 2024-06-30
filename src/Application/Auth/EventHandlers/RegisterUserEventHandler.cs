using SmartFridgeManagerAPI.Domain.Entities.Events;

namespace SmartFridgeManagerAPI.Application.Auth.EventHandlers;

public class RegisterUserEventHandler : INotificationHandler<RegisterUserEvent>
{
    public Task Handle(RegisterUserEvent notification, CancellationToken cancellationToken)
    {
        Log.Logger.Information("SmartFridgeManagerAPI Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
