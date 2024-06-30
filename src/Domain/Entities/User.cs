using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
    public virtual ActivationToken? ActivationToken { get; init; }
    public virtual List<ResetPasswordToken>? ResetPasswordTokens { get; init; }
}
