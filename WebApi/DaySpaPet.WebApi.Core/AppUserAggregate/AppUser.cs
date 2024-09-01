using Ardalis.GuardClauses;
using DaySpaPet.WebApi.Core.AppUserAggregate.Events;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.SharedKernel.GuardClauses;
using NodaTime;

namespace DaySpaPet.WebApi.Core.AppUserAggregate;

// TODO: Create a Roslyn Analyzer for EntityBase to ensure that Entity Framework
// can instantiate using either a parameterless constructor or a constructor with
// parameters for the properties (except for the Id property and any navigation properties).
public class AppUser : EntityBase<Guid>, IAggregateRoot {
  // Required
  public string PasswordHash { get; internal set; }
  public string PasswordSalt { get; internal set; }
  public string HashingAlgorithm { get; internal set; }
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
    Id = Guid.NewGuid();
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

    AppUserCreatedEvent domainEvent = new(
      Id,
      Username, EmailAddress, 
      TimeZoneId, Locale, Currency, 
      FirstName, LastName, MiddleName,
      DateOfBirth, ProfileImageUrl, PhoneNumber, AddressLine1, AddressLine2, City, State, PostalCode, CountryCode, AdministratorId, originClock);
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

    SetModifiedAt(originClock);

    AppUserNameUpdatedEvent domainEvent = new(Id, Username, EmailAddress, newFirstName, newLastName, newMiddleName, oldFirstName, oldLastName, oldMiddleName, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdatePhone(string newPhoneNumber, OriginClock originClock) {
    string oldPhoneNumber = newPhoneNumber;
    PhoneNumber = Guard.Against.NullOrEmpty(newPhoneNumber, nameof(newPhoneNumber)).Trim();

    SetModifiedAt(originClock);

    AppUserPhoneUpdatedEvent domainEvent = new(Id, Username, EmailAddress, newPhoneNumber, oldPhoneNumber, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateRegionalInformation(string newTimeZoneId, string newLocale, string newCurrency, OriginClock originClock) {
    string oldTimeZoneId = TimeZoneId;
    string oldLocale = Locale;
    string oldCurrency = Currency;
    TimeZoneId = Guard.Against.NullOrEmpty(newTimeZoneId, nameof(newTimeZoneId)).Trim();
    Locale = Guard.Against.NullOrEmpty(newLocale, nameof(newLocale)).Trim();
    Currency = Guard.Against.NullOrEmpty(newCurrency, nameof(newCurrency)).Trim();

    SetModifiedAt(originClock);

    AppUserLocationUpdatedEvent domainEvent = new(Id, Username, EmailAddress, newTimeZoneId, newLocale, newCurrency, oldTimeZoneId, oldLocale, oldCurrency, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateAddress(string? newAddressLine1, string? newAddressLine2, string? newCity, string? newState, string? newPostalCode, string? newCountryCode, OriginClock originClock) {
    string? oldAddressLine1 = AddressLine1;
    string? oldAddressLine2 = AddressLine2;
    string? oldCity = City;
    string? oldState = State;
    string? oldPostalCode = PostalCode;
    string? oldCountryCode = CountryCode;

    AddressLine1 = newAddressLine1?.Trim();
    AddressLine2 = newAddressLine2?.Trim();
    City = newCity?.Trim();
    State = newState?.Trim();
    PostalCode = newPostalCode?.Trim();
    CountryCode = newCountryCode?.Trim();

    SetModifiedAt(originClock);

    AppUserAddressUpdatedEvent domainEvent = new(Id, Username, EmailAddress, newAddressLine1, newAddressLine2, newCity, newState, newPostalCode, newCountryCode, oldAddressLine1, oldAddressLine2, oldCity, oldState, oldPostalCode, oldCountryCode, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void UpdateEmailAddress(string newEmailAddress, OriginClock originClock) {
    Guard.Against.NullOrEmpty(newEmailAddress, nameof(newEmailAddress));
    string? oldEmailAddress = EmailAddress;

    EmailAddress = Guard.Against.EmailInvalid(newEmailAddress.Trim());

    SetModifiedAt(originClock);

    AppUserEmailAddressUpdatedEvent domainEvent = new(Id, Username, EmailAddress, newEmailAddress, oldEmailAddress, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void GrantAdministrativeAccess(OriginClock originClock) {
    AdministratorId = Guid.NewGuid();

    SetModifiedAt(originClock);

    AppUserGrantedAdministrativeAccessEvent domainEvent = new(Id, Username, EmailAddress, AdministratorId!.Value, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void RevokeAdministrativeAccess(OriginClock originClock) {
    Guid? oldAdministratorId = AdministratorId;
    AdministratorId = null;

    SetModifiedAt(originClock);

    AppUserRevokedAdministrativeAccessEvent domainEvent = new(Id, Username, EmailAddress, oldAdministratorId!.Value, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void AddAppUserRole(AppUserRole appUserRole,
          OriginClock originClock) {
    _userRoles.Add(new(Id, appUserRole.Id, originClock));

    SetModifiedAt(originClock);

    AppUserAssignedRoleCreatedEvent domainEvent = new(Id, Username, EmailAddress, appUserRole.Id, appUserRole.ShortName, appUserRole.LongName, originClock);
    RegisterDomainEvent(domainEvent);
  }

  public void RemoveAppUserRole(AppUserRole appUserRole,
          OriginClock originClock) {
    AppUserAssignedRole? auar = _userRoles
        .SingleOrDefault(r => r.AppUserId == Id && r.AppUserRoleId == appUserRole.Id) 
      ?? throw new InvalidOperationException("User does not have the specified role.");

    _userRoles.Remove(auar);

    SetModifiedAt(originClock);

    AppUserAssignedRoleRemovedEvent domainEvent = new(Id, Username, EmailAddress, appUserRole.Id, appUserRole.ShortName, appUserRole.LongName, originClock);
    RegisterDomainEvent(domainEvent);
  }
}