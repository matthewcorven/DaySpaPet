using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserNameUpdatedEvent(
        Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        string NewFirstName, string NewLastName, string? NewMiddleName, string OldFirstName, string OldLastName, string? OldMiddleName,
        OriginClock OriginClock) : DomainEventBase(OriginClock);