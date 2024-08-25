using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserPhoneUpdatedEvent(
        Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        string? NewPhoneNumber, string? OldPhoneNumber,
        OriginClock OriginClock) : DomainEventBase(OriginClock);