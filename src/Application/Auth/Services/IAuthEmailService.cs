namespace SmartFridgeManagerAPI.Application.Auth.Services;

public interface IAuthEmailService
{
    void SendActivationEmail(string userEmail, string activationTokenRedirectUrl, string activationToken);
}
