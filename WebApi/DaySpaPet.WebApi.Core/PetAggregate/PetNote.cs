using Ardalis.GuardClauses;
using DaySpaPet.WebApi.SharedKernel;
using EntityBase = DaySpaPet.WebApi.SharedKernel.EntityBase;

namespace DaySpaPet.WebApi.Core.PetAggregate;

public class PetNote : EntityBase {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public PetNote()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public PetNote(int petId, string text, bool isAlert, OriginClock originClock) {
    PetId = Guard.Against.Negative(petId, nameof(petId));
    Text = Guard.Against.NullOrEmpty(text, nameof(text));
    IsAlert = isAlert;

    this.SetCreatedAt(originClock);
  }

  public int PetId { get; private set; }
  public string Text { get; private set; }
  public bool IsAlert { get; private set; }
}