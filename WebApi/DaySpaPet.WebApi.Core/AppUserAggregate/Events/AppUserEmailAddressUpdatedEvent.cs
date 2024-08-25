using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserEmailAddressUpdatedEvent(
  Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        string? NewEmailAddress, string? OldEmailAddress,
        OriginClock OriginClock) : DomainEventBase(OriginClock);