using DaySpaPet.WebApi.SharedKernel;
using NodaTime;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Events;

public record AppUserCreatedEvent(
      Guid Id,
      string Username, string EmailAddress,
      string TimeZoneId, string Locale, string Currency,
      string FirstName, string LastName, string? MiddleName,
      AnnualDate? DateOfBirth, string? ProfileImageUrl, string? PhoneNumber, string? AddressLine1, string? AddressLine2, string? City, string? State, string? PostalCode, string? CountryCode, Guid? AdministratorId, OriginClock OriginClock) : DomainEventBase(OriginClock);