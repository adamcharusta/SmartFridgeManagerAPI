using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartFridgeManagerAPI.Domain.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Interceptors;

public class BaseEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (EntityEntry<BaseEntity> entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                DateTimeOffset utcNow = timeProvider.GetUtcNow();
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                }

                entry.Entity.ModifyAt = utcNow;
            }
        }
    }
}
