namespace DaySpaPet.UseCases.Settings.Domain;
public record KernelSettings
{
    public DebugSettings DebugSettings { get; set; } = null!;
}
