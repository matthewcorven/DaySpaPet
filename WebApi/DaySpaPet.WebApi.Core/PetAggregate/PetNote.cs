using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.PetAggregate;

public class PetNote : EntityBase
{
  #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public PetNote()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public PetNote(int petId, string text, CapturedDateTime date, bool isAlert)
  {
    PetId = Guard.Against.Negative(petId, nameof(petId));
    Text = Guard.Against.NullOrEmpty(text, nameof(text));
    CreatedAt = date;
    IsAlert = isAlert;
  }

  public int PetId { get; private set; }
  public string Text { get; private set; }
  public CapturedDateTime CreatedAt { get; private set; }
  public bool IsAlert { get; private set; }
}
