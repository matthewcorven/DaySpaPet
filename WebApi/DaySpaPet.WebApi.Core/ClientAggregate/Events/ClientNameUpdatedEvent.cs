using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;
public record ClientNameUpdatedEvent(
        int ClientId,
        string NewFirstName, string NewLastName,
        string OldFirstName, string OldLastName,
        OriginClock OriginClock)
        : DomainEventBase(OriginClock);