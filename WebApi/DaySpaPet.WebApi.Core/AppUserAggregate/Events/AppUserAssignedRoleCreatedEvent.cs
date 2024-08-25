using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserAssignedRoleCreatedEvent(
  Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        Guid RoleId,
        string ShortName,
        string LongName,
        OriginClock OriginClock) : DomainEventBase(OriginClock);