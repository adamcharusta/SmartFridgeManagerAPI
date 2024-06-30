namespace SmartFridgeManagerAPI.Application.Auth.Settings;

public class AuthSettings
{
    public required string JwtKey { get; set; }
    public int JwtExpireDays { get; set; }
    public required string JwtIssuer { get; set; }
}
