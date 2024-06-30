namespace SmartFridgeManagerAPI.Application.Auth.Services;

public interface IAuthEmailService
{
    void SendActivationEmail(string userEmail, string activationToken);
}
