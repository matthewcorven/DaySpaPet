using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record AppUserAddressUpdatedEvent(
        Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        string? NewAddressLine1, string? NewAddressLine2, 
        string? NewCity, string? NewState, string? NewPostalCode, string? NewCountryCode, 
        string? OldAddressLine1, string? OldAddressLine2, 
        string? OldCity, string? OldState, string? OldPostalCode, string? OldCountryCode,
        OriginClock OriginClock) : DomainEventBase(OriginClock);