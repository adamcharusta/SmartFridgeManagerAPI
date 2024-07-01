using Microsoft.Extensions.Localization;
using SmartFridgeManagerAPI.Domain.Queues;
using SmartFridgeManagerAPI.Infrastructure.Common.RabbitMq;

namespace SmartFridgeManagerAPI.Application.Auth.Services;

public class AuthEmailService(IRabbitMqService rabbitMqService, IStringLocalizer<Messages> messages)
    : IAuthEmailService
{
    public void SendActivationEmail(string userEmail, string activationTokenRedirectUrl, string activationToken)
    {
        string body =
            $"""{messages["SendActivationEmailBody"]}: <br/> <a href="{BuildUrl([activationTokenRedirectUrl, activationToken])}">{messages["SendActivationEmailBodyBtn"]}!</a>""";

        EmailQueue queue = new(userEmail, messages["SendActivationEmailSubject"], body);

        rabbitMqService.BasicPublish(queue);
    }

    private string BuildUrl(string[] path)
    {
        char separateSymbol = '/';
        List<string> result = [];
        foreach (string s in path)
        {
            result.Add(s.TrimEnd(separateSymbol));
        }

        return string.Join(separateSymbol, result);
    }
}
