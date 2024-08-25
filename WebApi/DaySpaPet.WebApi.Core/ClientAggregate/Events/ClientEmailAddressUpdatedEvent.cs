using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

public sealed record ClientEmailAddressUpdatedEvent(
        int ClientId, 
        string NewEmailAddress, string? OldEmailAddress, 
        OriginClock OriginClock) : DomainEventBase(OriginClock);