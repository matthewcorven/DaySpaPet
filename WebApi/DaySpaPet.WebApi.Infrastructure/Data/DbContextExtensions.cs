using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace DaySpaPet.WebApi.Infrastructure.Data;

public static class DbContextExtensions
{
	public static bool HasTruthySectionValue(this IConfigurationSection cfgSection, string varName)
	{
		Guard.Against.Null(cfgSection, nameof(cfgSection));
		var gottenSection = cfgSection!.GetSection(varName);
		Guard.Against.Null(gottenSection, nameof(gottenSection));
		Guard.Against.NullOrEmpty(gottenSection!.Value, $"{nameof(gottenSection)}.Value");
		_ = bool.TryParse(gottenSection!.Value, out var gottenSectionValue);
		return gottenSectionValue == true;
	}
}
