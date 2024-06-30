namespace SmartFridgeManagerAPI.Infrastructure.Settings;

public class RabbitMqSettings
{
    public int Port { get; set; }
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
