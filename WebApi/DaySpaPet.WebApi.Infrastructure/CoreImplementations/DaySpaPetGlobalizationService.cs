using DaySpaPet.WebApi.Core.Interfaces;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace DaySpaPet.WebApi.Infrastructure.CoreImplementations;

public class DaySpaPetGlobalizationService : IGlobalizationService {
  private readonly ILogger<DaySpaPetGlobalizationService> _logger;

  public DaySpaPetGlobalizationService(ILogger<DaySpaPetGlobalizationService> logger) {
    _logger = logger;
  }

  public bool TryGetTimeZoneById(string timeZoneId, out DateTimeZone? provider) {
    provider = null;
    try {
      provider = DateTimeZoneProviders.Tzdb[timeZoneId];
      return true;
    } catch (Exception ex) {
      _logger.LogDebug(ex, "Failed to get time zone by ID: {TimeZoneId}", timeZoneId);
      return false;
    }
  }
}