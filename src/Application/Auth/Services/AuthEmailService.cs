using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.Infrastructure.Common.Services;

namespace SmartFridgeManagerAPI.Application.Auth.Services;

public class AuthEmailService(IRabbitMqService rabbitMqService)
    : IAuthEmailService
{
    public void SendActivationEmail(string userEmail, string activationToken)
    {
        EmailQueue queue = new(userEmail, "Activate your account", activationToken);

        rabbitMqService.BasicPublish(queue);
    }
}
