using SmartFridgeManagerAPI.Domain.Entities;

namespace SmartFridgeManagerAPI.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ActivationToken> ActivationTokens => Set<ActivationToken>();
    public DbSet<ResetPasswordToken> ResetPasswordTokens => Set<ResetPasswordToken>();
}
