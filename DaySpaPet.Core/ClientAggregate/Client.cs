using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using DaySpaPet.Core.ClientAggregate.Events;
using DaySpaPet.Core.PetAggregate;
using DaySpaPet.SharedKernel;
using DaySpaPet.SharedKernel.GuardClauses;

namespace DaySpaPet.Core.ClientAggregate;

public sealed class Client : EntityBase, IAggregateRoot
{
  // Required
  public string FirstName { get; private set; } = default!;
  public string LastName { get; private set; } = default!;
  public string PhoneCountryCode { get; private set; } = string.Empty;
  public string PhoneNumber { get; private set; } = string.Empty;
  public string? PhoneExtension { get; private set; } = string.Empty;
  public ClientAccountStatus Status { get; private set; } = ClientAccountStatus.NotSet;
  // Timestamps
  public CapturedDateTime CreatedAt { get; private set; }
  // Optional
  public string? EmailAddress { get; private set; }
  // Navigation
  private readonly List<Pet> _pets = new();
  public IEnumerable<Pet> Pets => _pets.AsReadOnly();

  public Client(string firstName, string lastName, string? phoneCountryCode, string phoneNumber, string? phoneExtension, string emailAddress, CapturedDateTime capturedDateTime)
  {
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    PhoneCountryCode = phoneCountryCode ?? Defaults.DefaultPhoneCountryCode;
    PhoneNumber = Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber));
    PhoneExtension = phoneExtension ?? null;
    EmailAddress = Guard.Against.EmailInvalid(Guard.Against.NullOrEmpty(emailAddress, nameof(emailAddress)));
    CreatedAt = capturedDateTime;
  }

  public void UpdateName(string newFirstName, string newLastName)
  {
    var oldFirstName = FirstName;
    var oldLastName = LastName;
    FirstName = Guard.Against.NullOrEmpty(newFirstName, nameof(newFirstName)).Trim();
    LastName = Guard.Against.NullOrEmpty(newLastName, nameof(newLastName)).Trim();

    var domainEvent = new ClientNameUpdatedEvent(this, newFirstName, newLastName, oldFirstName, oldLastName);
    base.RegisterDomainEvent(domainEvent);
  }

  public void UpdatePhoneNumber(string? phoneCountryCode, string phoneNumber, string? phoneExtension)
  {
    PhoneCountryCode = phoneCountryCode ?? Defaults.DefaultPhoneCountryCode;
    PhoneNumber = Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber)).Trim();
    PhoneExtension = phoneExtension ?? null;
  }

  public void UpdateEmailAddress(string newEmailAddress)
  {
    string? oldEmailAddress = EmailAddress;
    Guard.Against.NullOrEmpty(newEmailAddress, nameof(newEmailAddress));

    EmailAddress = Guard.Against.EmailInvalid(newEmailAddress.Trim());

    var domainEvent = new ClientEmailAddressUpdatedEvent(this, newEmailAddress, oldEmailAddress);
    base.RegisterDomainEvent(domainEvent);
  }

  public void SetStatus(ClientAccountStatus newStatus)
  {
    Status = Guard.Against.NullOrInvalidInput(newStatus, nameof(newStatus), ClientAccountStatus.IsNotSet);
  }

  public void AddPet(string newPetName, AnimalType newAnimalType, string newPetBreed, CapturedDateTime capturedDateTime, OptionalNewPetData? optionalNewPetData = null)
  {
    Guard.Against.NullOrEmpty(newPetName, nameof(newPetName));
    Guard.Against.NullOrInvalidInput(newAnimalType, nameof(newAnimalType), AnimalType.IsNotSet);
    Guard.Against.NullOrEmpty(newPetBreed, nameof(newPetBreed));

    var pet = new Pet(newPetName, newAnimalType, newPetBreed, capturedDateTime, optionalNewPetData);
    _pets.Add(pet);
  }
}
