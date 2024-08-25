using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;
public record class AppUserGrantedAdministrativeAccessEvent(
  Guid AppUserId,
        string AppUserUsername,
        string AppUserEmailAddress,
        Guid AdministratorId,
        OriginClock OriginClock) : DomainEventBase(OriginClock);