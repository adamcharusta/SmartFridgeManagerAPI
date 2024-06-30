using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Domain.Entities.Events;

public class RegisterUserEvent(User user) : BaseEvent
{
    public User User { get; set; } = user;
}
