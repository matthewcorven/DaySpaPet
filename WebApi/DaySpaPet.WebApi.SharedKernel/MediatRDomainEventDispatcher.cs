using MediatR;

namespace DaySpaPet.WebApi.SharedKernel;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher {
  private readonly IMediator _mediator;

  public MediatRDomainEventDispatcher(IMediator mediator) {
    _mediator = mediator;
  }

  public async Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents) {
    foreach (EntityBase entity in entitiesWithEvents) {
      DomainEventBase[] events = entity.DomainEvents.ToArray();
      entity.ClearDomainEvents();
      foreach (DomainEventBase? domainEvent in events) {
        await _mediator.Publish(domainEvent).ConfigureAwait(false);
      }
    }
  }

  public async Task DispatchAndClearEvents<TId>(IEnumerable<EntityBase<TId>> entitiesWithEvents)
          where TId : struct, IEquatable<TId> {
    foreach (EntityBase<TId> entity in entitiesWithEvents) {
      DomainEventBase[] events = entity.DomainEvents.ToArray();
      entity.ClearDomainEvents();
      foreach (DomainEventBase? domainEvent in events) {
        await _mediator.Publish(domainEvent).ConfigureAwait(false);
      }
    }
  }
}