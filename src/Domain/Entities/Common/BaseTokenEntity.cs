namespace SmartFridgeManagerAPI.Domain.Entities.Common;

public abstract class BaseTokenEntity(bool isActive, DateTimeOffset expiresAt) : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public string Token { get; set; } = GetToken();
    public bool IsActive { get; set; } = isActive;
    public DateTimeOffset ExpiresAt { get; set; } = expiresAt;

    protected static string GetToken()
    {
        return Guid.NewGuid().ToString();
    }
}
