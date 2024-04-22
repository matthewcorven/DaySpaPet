using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using DaySpaPet.SharedKernel.GuardClauses;
using DaySpaPet.Core.PetAggregate.Events;
using DaySpaPet.SharedKernel;

namespace DaySpaPet.Core.PetAggregate;

public sealed record OptionalNewPetData(double? Weight, int? Age, DateOnly? BirthDate, DateOnly? AdoptionDate);

public sealed class Pet : EntityBase, IAggregateRoot
{
  // Required
  public int ClientId { get; private set; }
  public string Name { get; private set; } = default!;
  public AnimalType Type { get; private set; }
  public string Breed { get; private set; } = default!;
  public PetStatus Status { get; private set; } = PetStatus.NotSet;
  // Timestamps
  public CapturedDateTime CreatedAt { get; private set; }
  // Optional
  public double? Weight { get; private set; }
  public int? Age { get; private set; }
  public DateOnly? BirthDate { get; private set; }
  public DateOnly? DeathDate { get; private set; }
  public DateOnly? AdoptionDate { get; private set; }
  public DateOnly? MostRecentVisitDate { get; private set; }
  public DateOnly? FirstVisitDate { get; private set; }
  // Navigation
  private readonly List<PetNote> _notes = new();

  public Pet(string name, AnimalType type, string breed, CapturedDateTime createdAt, OptionalNewPetData? optionalNewPetInfo = null)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    Type = type;
    Breed = Guard.Against.NullOrEmpty(breed, nameof(breed));
    Status = PetStatus.New;
    CreatedAt = createdAt;
    if (optionalNewPetInfo is not null)
    {
      SetWeight(optionalNewPetInfo.Weight);
      // If both BirthDate and Age are provided, BirthDate takes precedence
      if (optionalNewPetInfo.BirthDate is not null)
      {
        SetBirthDateAndCalculateExactAge(optionalNewPetInfo.BirthDate.Value);
      }
      else if (optionalNewPetInfo.Age is not null)
      {
        SetAgeAndApproximateBirthDate(optionalNewPetInfo.Age.Value);
      }
      SetAdoptionDate(optionalNewPetInfo.AdoptionDate);
    }
  }

  public IEnumerable<PetNote> Notes => _notes.AsReadOnly();

  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }

  public void UpdateBreed(string newBreed)
  {
    Breed = Guard.Against.NullOrEmpty(newBreed, nameof(newBreed));
  }

  public void SetStatus(PetStatus newStatus)
  {
    Status = Guard.Against.NullOrInvalidInput(newStatus, nameof(newStatus), PetStatus.IsNotSet);
  }

  public void SetWeight(double? newWeight)
  {
    Weight = newWeight is not null
      ? Guard.Against.NegativeOrZero(newWeight.Value, nameof(newWeight))
      : null;
  }

  public void SetAgeAndApproximateBirthDate(int newAge)
  {
    Age = Guard.Against.Negative(newAge, nameof(newAge));
    var approxBirthDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * newAge * 365.25));
    BirthDate = approxBirthDate;
  }

  public void SetBirthDateAndCalculateExactAge(DateOnly newBirthDate)
  {
    BirthDate = Guard.Against.DateOnlyIsNotAfter(newBirthDate,
           DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * Defaults.DefaultMinimumPetAgeDays)),
                nameof(newBirthDate));
    Age = (int)Math.Floor((DateTime.Today - newBirthDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now))).TotalDays / 365.25);
  }

  public void SetDeathDate(DateOnly? newDeathDate)
  {
    if (newDeathDate is not null)
    {
      DeathDate = newDeathDate is not null
        ? Guard.Against.DateOnlyIsNotAfter(newDeathDate.Value,
          DateOnly.FromDateTime(DateTime.Today),
          nameof(newDeathDate))
        : null;
      if (newDeathDate is not null)
      {
        SetStatus(PetStatus.Inactive);
      }
    }
  }

  public void SetAdoptionDate(DateOnly? newAdoptionDate)
  {
    AdoptionDate = newAdoptionDate;
  }

  public void SetMostRecentVisitDate(DateOnly? newMostRecentVisitDate)
  {
    if (newMostRecentVisitDate is not null)
    {
      Guard.Against.DateOnlyIsNotAfter(newMostRecentVisitDate.Value, DateOnly.FromDateTime(DateTime.Today), nameof(newMostRecentVisitDate));

      if (FirstVisitDate is not null)
      {
        MostRecentVisitDate = Guard.Against.DateOnlyIsNotBefore(newMostRecentVisitDate.Value, FirstVisitDate.Value, nameof(newMostRecentVisitDate));
      }
    }
    else
    {
      MostRecentVisitDate = null;
    }
  }

  public void SetFirstVisitDate(DateOnly? newFirstVisitDate)
  {
    if (newFirstVisitDate is not null)
    {
      Guard.Against.DateOnlyIsNotAfter(newFirstVisitDate.Value, DateOnly.FromDateTime(DateTime.Today), nameof(newFirstVisitDate));

      if (MostRecentVisitDate is not null)
      {
        FirstVisitDate = Guard.Against.DateOnlyIsNotAfter(newFirstVisitDate.Value, MostRecentVisitDate.Value, nameof(newFirstVisitDate));
      }
    }
    else
    {
      FirstVisitDate = null;
    }
  }

  public void AddNote(PetNote newNote)
  {
    Guard.Against.Null(newNote, nameof(newNote));
    _notes.Add(newNote);

    var newNoteAddedEvent = new NewNoteAddedEvent(this, newNote);
    base.RegisterDomainEvent(newNoteAddedEvent);
  }

}
