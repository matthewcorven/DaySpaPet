using Microsoft.Extensions.Configuration;

namespace DaySpaPet.WebApi.SharedKernel.Extensions;
public static class IConfigurationExtensions {
  public static bool TryGetRequiredConfiguration(this IConfiguration configSection, string key, out string? value) {
    value = configSection[key];
    if (string.IsNullOrWhiteSpace(value)) {
      //_logger.Information($"{key} is not set in configuration via appsettings at \"{configSection.Path}:{key}\", nor overridden by Environment Variable key \"{key.Replace(":", "__")}\"");
      return false;
    }
    return true;
  }

  public static bool TryGetRequiredConfiguration(this IConfigurationSection configSection, string key, out string? value) {
    value = configSection[key];
    if (string.IsNullOrWhiteSpace(value)) {
      //_logger.Information($"{key} is not set in configuration via appsettings at \"{configSection.Path}:{key}\", nor overridden by Environment Variable key \"{key.Replace(":", "__")}\"");
      return false;
    }
    return true;
  }
  
  public static bool TryGetRequiredConfiguration<TOut>(this IConfiguration configSection, string key, out TOut? value) {
    value = default!;

    if (int.TryParse(configSection[key], out int intValue)) {
      value = (TOut)(object)intValue;
      return true;
    } else {
      return false;
    }
  }

  public static bool TryGetRequiredConfiguration<TOut>(this IConfiguration configSection, Serilog.ILogger logger, string key, out TOut? value) {
    value = default!;

    if (int.TryParse(configSection[key], out int intValue)) {
      value = (TOut)(object)intValue;
      return true;
    } else {
      logger.Fatal("Unable to read value for required section key {sectionName}:{keyName}", configSection.ToString(), key);
      return false;
    }
  }
}
