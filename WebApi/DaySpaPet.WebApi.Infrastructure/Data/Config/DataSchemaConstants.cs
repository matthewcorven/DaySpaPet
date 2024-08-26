namespace DaySpaPet.Infrastructure.Data.Config;
#pragma warning disable CA1707
public static class DataSchemaConstants {
  private static int GetIntFromEnv(string envVarName, int defaultValue) {
    return int.Parse(Environment.GetEnvironmentVariable($"DSP_SCHEMA_{envVarName}") ?? defaultValue.ToString());
  }
  public static readonly int DEFAULT_NAME_LENGTH = GetIntFromEnv(nameof(DEFAULT_NAME_LENGTH), 100);
  public static readonly int DEFAULT_URL_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 2048); // https://www.baeldung.com/cs/max-url-length
  public static readonly int DEFAULT_EMAIL_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 254); // http://www.rfc-editor.org/errata_search.php?rfc=3696
  public static readonly int DEFAULT_PHONE_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 25);
  public static readonly int DEFAULT_ADDRESS_LINE_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 100);
  public static readonly int DEFAULT_SHORT_NAME_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 50);
  public static readonly int DEFAULT_POSTAL_CODE_LENGTH = GetIntFromEnv(nameof(DEFAULT_URL_LENGTH), 30);
  public const int DEFAULT_ENUM_STRING_LENGTH = 1; 
}
#pragma warning restore CA1707