namespace DaySpaPet.WebApi.UseCases.Settings.Domain;

public record DebugSettings {
  public int KeyOne { get; set; }
  public bool KeyTwo { get; set; }
  public NestedSettings KeyThree { get; set; } = null!;
}