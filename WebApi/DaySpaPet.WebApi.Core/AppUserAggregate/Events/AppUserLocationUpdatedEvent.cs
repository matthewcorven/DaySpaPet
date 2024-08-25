using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserLocationUpdatedEvent(
        Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        string NewTimeZoneId, string NewLocale, string NewCurrency, 
        string OldTimeZoneId, string OldLocale, string OldCurrency,
        OriginClock OriginClock) : DomainEventBase(OriginClock);