using Ardalis.GuardClauses;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.ClientAggregate.Events;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.SharedKernel.GuardClauses;
using NodaTime;
using System.Net.Mail;
using System;

namespace DaySpaPet.WebApi.Core.AppUserAggregate;

// TODO: Create a Roslyn Analyzer for EntityBase to ensure that Entity Framework
// can instantiate using either a parameterless constructor or a constructor with
// parameters for the properties (except for the Id property and any navigation properties).
public class AppUser : EntityBase<Guid>, IAggregateRoot {
  // Required
  public string PasswordHash { get; internal set; }
  public string PasswordSalt { get; internal set; }
  public string HashingAlgorithm { get; internal set; }
  // TODO: Ensure unique across tenant
  public string Username { get; internal set; }
  public string EmailAddress { get; internal set; }
  public string TimeZoneId { get; internal set; }
  public string Locale { get; internal set; }
  public string Currency { get; internal set; }
  public string FirstName { get; internal set; }
  public string LastName { get; internal set; }
  // Optional
  public string? MiddleName { get; internal set; }
  public AnnualDate? DateOfBirth { get; internal set; }
  public string? ProfileImageUrl { get; internal set; }
  public string? PhoneNumber { get; internal set; }
  public string? AddressLine1 { get; internal set; }
  public string? AddressLine2 { get; internal set; }
  public string? City { get; internal set; }
  public string? State { get; internal set; }
  public string? PostalCode { get; internal set; }
  public string? CountryCode { get; internal set; }
  public Guid? AdministratorId { get; internal set; }
  // Navigation
  private readonly List<AppUserAssignedRole> _userRoles = [];
  public IEnumerable<AppUserAssignedRole> UserRoles => _userRoles.AsReadOnly();
  // Derived
  public string Name =>
    $"{FirstName} {MiddleName ?? string.Empty} {LastName}"
    .Replace("  ", " ");
  public string Address =>
    $"{AddressLine1 ?? string.Empty}{Environment.NewLine}{AddressLine2 ?? string.Empty}".Trim();
  public bool IsAdministrator => AdministratorId is not null;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public AppUser()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF

  }

  public AppUser(string passwordHash
        ,string passwordSalt
        ,string hashingAlgorithm
        ,string username
        ,string emailAddress
        ,string timeZoneId
        ,string locale
        ,string currency
        ,string firstName
        ,string lastName
        ,string? middleName
        ,AnnualDate? dateOfBirth
        ,string? profileImageUrl
        ,string? phoneNumber
        ,string? addressLine1
        ,string? addressLine2
        ,string? city
        ,string? state
        ,string? postalCode
        ,string? countryCode
        ,Guid? administratorId
        ,OriginClock originClock) {
    PasswordHash = Guard.Against.NullOrEmpty(passwordHash, nameof(passwordHash));
    PasswordSalt = Guard.Against.NullOrEmpty(passwordSalt, nameof(passwordSalt));
    HashingAlgorithm = Guard.Against.NullOrEmpty(hashingAlgorithm, nameof(hashingAlgorithm));
    Username = Guard.Against.NullOrEmpty(username, nameof(username));
    EmailAddress = Guard.Against.NullOrEmpty(emailAddress, nameof(emailAddress));
    TimeZoneId = Guard.Against.NullOrEmpty(timeZoneId, nameof(timeZoneId));
    Locale = Guard.Against.NullOrEmpty(locale, nameof(locale));
    Currency = Guard.Against.NullOrEmpty(currency, nameof(currency));
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));

    // Ensure that username is not equal to emailAddress
    if (Username.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase)) {
      throw new ArgumentException("Username cannot be the same as the email address.");
    }

    MiddleName = middleName;
    DateOfBirth = dateOfBirth;
    ProfileImageUrl = profileImageUrl;
    PhoneNumber = phoneNumber;
    AddressLine1 = addressLine1;
    AddressLine2 = addressLine2;
    City = city;
    State = state;
    PostalCode = postalCode;
    CountryCode = countryCode;
    AdministratorId = administratorId;

    SetCreatedAt(originClock);

    AppUserCreateEvent domainEvent = new AppUserCreateEvent(this, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateName(string newFirstName, string newLastName, string? newMiddleName, OriginClock originClock) {
    string oldFirstName = FirstName;
    string? oldMiddleName = MiddleName;
    string oldLastName = LastName;
    FirstName = Guard.Against.NullOrEmpty(newFirstName, nameof(newFirstName)).Trim();
    LastName = Guard.Against.NullOrEmpty(newLastName, nameof(newLastName)).Trim();
    if (newMiddleName is not null) {
      MiddleName = newMiddleName!;
    }

    AppUserNameUpdatedEvent domainEvent = new AppUserNameUpdatedEvent(this, newFirstName, newLastName, newMiddleName, oldFirstName, oldLastName, oldMiddleName, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdatePhone(string? phoneCountryCode, string phoneNumber, string? phoneExtension) {
    PhoneNumber = Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber)).Trim();
  }

  public void UpdateRegionalInformation(string newTimeZoneId, string newLocale, string newCurrency, OriginClock originClock) {
    string oldTimeZoneId = TimeZoneId;
    string oldLocale = Locale;
    string oldCurrency = Currency;
    TimeZoneId = Guard.Against.NullOrEmpty(newTimeZoneId, nameof(newTimeZoneId)).Trim();
    Locale = Guard.Against.NullOrEmpty(newLocale, nameof(newLocale)).Trim();
    Currency = Guard.Against.NullOrEmpty(newCurrency, nameof(newCurrency)).Trim();

    AppUserLocationUpdatedEvent domainEvent = new AppUserLocationUpdatedEvent(this, newTimeZoneId, newLocale, newCurrency, oldTimeZoneId, oldLocale, oldCurrency, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateAddress(string? addressLine1, string? addressLine2, string? city, string? state, string? postalCode, string? countryCode, OriginClock originClock) {
    string? oldAddressLine1 = AddressLine1;
    string? oldAddressLine2 = AddressLine2;
    string? oldCity = City;
    string? oldState = State;
    string? oldPostalCode = PostalCode;
    string? oldCountryCode = CountryCode;

    AddressLine1 = addressLine1?.Trim();
    AddressLine2 = addressLine2?.Trim();
    City = city?.Trim();
    State = state?.Trim();
    PostalCode = postalCode?.Trim();
    CountryCode = countryCode?.Trim();

    AppUserAddressUpdatedEvent domainEvent = new AppUserAddressUpdatedEvent(this, addressLine1, addressLine2, city, state, postalCode, countryCode, oldAddressLine1, oldAddressLine2, oldCity, oldState, oldPostalCode, oldCountryCode, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateEmailAddress(string newEmailAddress, OriginClock originClock) {
    Guard.Against.NullOrEmpty(newEmailAddress, nameof(newEmailAddress));
    string? oldEmailAddress = EmailAddress;

    EmailAddress = Guard.Against.EmailInvalid(newEmailAddress.Trim());

    AppUserEmailAddressUpdatedEvent domainEvent = new AppUserEmailAddressUpdatedEvent(this, newEmailAddress, oldEmailAddress, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void GrantAdministrativeAccess(OriginClock originClock) {
    AdministratorId = Guid.NewGuid();

    AppUserGrantedAdministrativeAccessEvent domainEvent = new AppUserGrantedAdministrativeAccessEvent(this, administratorId, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void AddAppUserRole(AppUserRole appUserRole,
          OriginClock originClock) {
    AppUserAssignedRole auar = new (this.Id, appUserRole.Id, originClock);
    _userRoles.Add(auar);

    AppUserAssignedRoleCreatedEvent domainEvent = new AppUserAssignedRoleCreatedEvent(this, appUserRole.Id, appUserRole.ShortName, appUserRole.LongName, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void RemoveAppUserRole(AppUserRole appUserRole,
          OriginClock originClock) {
    AppUserAssignedRole? auar = _userRoles.SingleOrDefault(r => r.AppUserId == this.Id && r.AppUserRoleId == appUserRole.Id);
    if (auar is null) {
      throw new InvalidOperationException("User does not have the specified role.");
    }

    _userRoles.Remove(auar);

    AppUserAssignedRoleRemovedEvent domainEvent = new AppUserAssignedRoleRemovedEvent(this, appUserRole.Id, appUserRole.ShortName, appUserRole.LongName, originClock);
    RegisterDomainEvent(domainEvent);
  }
}