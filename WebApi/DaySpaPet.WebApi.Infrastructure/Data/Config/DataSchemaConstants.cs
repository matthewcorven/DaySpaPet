namespace DaySpaPet.Infrastructure.Data.Config;
public static class DataSchemaConstants {
#pragma warning disable CA1707 // Identifiers should not contain underscores
  public static readonly int DEFAULT_NAME_LENGTH = int.Parse(Environment.GetEnvironmentVariable("") ?? "100");
  public const int DEFAULT_ENUM_STRING_LENGTH = 1;
#pragma warning restore CA1707 // Identifiers should not contain underscores
}