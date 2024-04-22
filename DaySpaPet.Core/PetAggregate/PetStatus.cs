using Ardalis.SmartEnum;

namespace DaySpaPet.Core.PetAggregate;

public class PetStatus : SmartEnum<PetStatus, string>
{
  public static readonly PetStatus NotSet = new(nameof(NotSet), Facts.NotSet);
  public static readonly PetStatus New = new(nameof(New), "N");
  public static readonly PetStatus Active = new(nameof(Active), "A");
  public static readonly PetStatus Inactive = new(nameof(Inactive), "I");

  public static Func<PetStatus> Default => () => NotSet;
  public static Func<PetStatus, bool> IsNotSet => _ => _ == NotSet;

  private PetStatus(string name, string value) : base(name, value)
  {
  }
}
