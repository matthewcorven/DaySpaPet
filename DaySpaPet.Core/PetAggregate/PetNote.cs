using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using DaySpaPet.SharedKernel;

namespace DaySpaPet.Core.PetAggregate;

public class PetNote(int petId, string text, CapturedDateTime date, bool isAlert) : EntityBase
{
  public int PetId { get; private set; } = Guard.Against.NegativeOrZero(petId, nameof(petId));
  public string Text { get; private set; } = Guard.Against.NullOrEmpty(text, nameof(text));
  public CapturedDateTime CreatedAt { get; private set; } = date;
  public bool IsAlert { get; private set; } = isAlert;
}
