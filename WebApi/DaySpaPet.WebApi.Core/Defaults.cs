using Ardalis.GuardClauses;

namespace DaySpaPet.WebApi.Core;

internal static class Defaults
{
  public static string DefaultPhoneCountryCode = default!;
  public static int DefaultMinimumPetAgeDays = default!;

  public static void EnsureIsFullyConfigured() {
    Guard.Against.NullOrEmpty(DefaultPhoneCountryCode, nameof(DefaultPhoneCountryCode));
    Guard.Against.Negative(DefaultMinimumPetAgeDays, nameof(DefaultMinimumPetAgeDays));
  }
}
