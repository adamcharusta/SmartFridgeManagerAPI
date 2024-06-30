using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public virtual ActivationToken? ActivationToken { get; set; }
    public virtual List<ResetPasswordToken>? ResetPasswordTokens { get; set; }
}
