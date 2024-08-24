﻿using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace DaySpaPet.WebApi.Infrastructure.Data;

public static class DbContextExtensions {
  public static bool HasTruthySectionValue(this IConfigurationSection cfgSection, string varName, out string value) {
    Guard.Against.Null(cfgSection, nameof(cfgSection));
    IConfigurationSection gottenSection = cfgSection!.GetSection(varName);
    Guard.Against.Null(gottenSection, nameof(gottenSection));
    Guard.Against.NullOrEmpty(gottenSection!.Value, $"{nameof(gottenSection)}.Value");
    _ = bool.TryParse(gottenSection!.Value, out bool gottenSectionValue);
    value = gottenSection!.Value;
    return gottenSectionValue == true;
  }
}