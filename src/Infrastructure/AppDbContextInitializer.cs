namespace SmartFridgeManagerAPI.Infrastructure;

public class AppDbContextInitializer(AppDbContext dbContext)
{
    public async Task InitialiseAsync()
    {
        if (dbContext.Database.IsRelational() && dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
