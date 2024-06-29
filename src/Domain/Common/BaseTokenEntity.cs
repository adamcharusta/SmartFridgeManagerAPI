using SmartFridgeManagerAPI.Domain.Entities;

namespace SmartFridgeManagerAPI.Domain.Common;

public class BaseTokenEntity : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public required string Token { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public string GetToken()
    {
        return Guid.NewGuid().ToString();
    }
}
