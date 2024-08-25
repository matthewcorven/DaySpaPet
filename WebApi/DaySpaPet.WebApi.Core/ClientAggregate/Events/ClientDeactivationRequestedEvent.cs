using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a client is deleted.
/// The DeactivateClientService is used to dispatch this event.
/// </summary>
public record ClientDeactivationRequestedEvent(
  int ClientId, 
  OriginClock OriginClock) : DomainEventBase(OriginClock);