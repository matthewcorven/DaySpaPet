using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using DaySpaPet.WebApi.Core.ClientAggregate.Events;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.SharedKernel.GuardClauses;

namespace DaySpaPet.WebApi.Core.ClientAggregate;

// TODO: Create a Roslyn Analyzer for EntityBase to ensure that Entity Framework
// can instantiate using either a parameterless constructor or a constructor with
// parameters for the properties (except for the Id property and any navigation properties).
public class Client : EntityBase, IAggregateRoot
{
  // Required
  public string FirstName { get; private set; } = default!;
  public string LastName { get; private set; } = default!;
  public string PhoneCountryCode { get; private set; } = default!;
  public string PhoneNumber { get; private set; } = default!;
  public string? PhoneExtension { get; private set; } = default!;
  public ClientAccountStatus Status { get; private set; } = ClientAccountStatus.NotSet;
  // Timestamps
  public CapturedDateTime CreatedAt { get; private set; }
  // Optional
  public string? EmailAddress { get; private set; }
  // Navigation
  private readonly List<Pet> _pets = [];
  public IEnumerable<Pet> Pets => _pets.AsReadOnly();

  #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public Client()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public Client(string firstName, string lastName, 
    string? phoneCountryCode, string phoneNumber, string? phoneExtension,
    string emailAddress, CapturedDateTime capturedDateTime)
  {
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    PhoneCountryCode = phoneCountryCode ?? Defaults.DefaultPhoneCountryCode;
    PhoneNumber = Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber));
    PhoneExtension = phoneExtension ?? null;
    Status = ClientAccountStatus.New;
    CreatedAt = capturedDateTime;
    EmailAddress = Guard.Against.EmailInvalid(Guard.Against.NullOrEmpty(emailAddress, nameof(emailAddress)));
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

    var pet = new Pet(clientId: this.Id, newPetName, newAnimalType, newPetBreed, capturedDateTime, optionalNewPetData);
    _pets.Add(pet);
  }
}
