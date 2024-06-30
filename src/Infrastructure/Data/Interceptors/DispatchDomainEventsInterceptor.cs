using MediatR;
using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        IEnumerable<BaseEntity> entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        IEnumerable<BaseEntity> baseEntities = entities as BaseEntity[] ?? entities.ToArray();
        List<BaseEvent> domainEvents = baseEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        baseEntities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (BaseEvent domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
