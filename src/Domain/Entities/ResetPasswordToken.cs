using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Domain.Entities;

public class ResetPasswordToken() : BaseTokenEntity(true, DateTimeOffset.Now.AddDays(1));
