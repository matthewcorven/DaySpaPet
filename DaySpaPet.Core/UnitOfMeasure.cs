using Ardalis.SmartEnum;

namespace DaySpaPet.Core;
public sealed class UnitOfMeasure : SmartEnum<UnitOfMeasure, string>
{
  public static readonly UnitOfMeasure Pounds = new(nameof(Pounds), "lbs");
  public static readonly UnitOfMeasure Kilograms = new(nameof(Kilograms), "kg");

  private UnitOfMeasure(string name, string value) : base(name, value) { }
}
