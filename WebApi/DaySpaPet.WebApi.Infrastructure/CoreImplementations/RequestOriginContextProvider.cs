using NodaTime;
using Ardalis.GuardClauses;
using DaySpaPet.WebApi.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Infrastructure.CoreImplementations;

public sealed class RequestOriginContextProvider : IRequestOriginContextProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IClock _clock;
  private readonly IGlobalizationService _globalizationService;

  public RequestOriginContextProvider(
    IHttpContextAccessor httpContextAccessor,
    IClock clock,
    IGlobalizationService globalizationService)
  {
    _httpContextAccessor = httpContextAccessor;
    _clock = clock;
    _globalizationService = globalizationService;
  }

  public OriginClock GetOriginClock()
  {
    var requestHeaders = _httpContextAccessor.HttpContext.Request.Headers;
    
    requestHeaders.TryGetValue("X-Origin-TimeZoneId", out var timeZoneIdStringValues);
    Guard.Against.Null(timeZoneIdStringValues, "HTTP request header \"X-Origin-TimeZoneId\" is required.");
    string? timeZoneIdValue = timeZoneIdStringValues.FirstOrDefault();
    Guard.Against.NullOrWhiteSpace(timeZoneIdValue, $"HTTP request header \"X-Origin-TimeZoneId\" value \"{timeZoneIdValue}\" is not understood to understand a string.");

    // Get the current UTC time
    Instant now = _clock.GetCurrentInstant();        
    // Get the time zone from the ID
    if (!_globalizationService.TryGetTimeZoneById(timeZoneIdValue, out DateTimeZone? zone))
    {
      throw new InvalidTimeZoneException($"Time zone ID \"{timeZoneIdValue}\" is not a valid time zone ID.");
    }

    // Get the ZonedDateTime in the specified zone
    var zonedDateTime = now.InZone(zone!);

    // Convert to LocalDateTime
    var originLocalDateTime = zonedDateTime.LocalDateTime;
    var isDst = zonedDateTime.IsDaylightSavingTime();

    return new OriginClock(originLocalDateTime, timeZoneIdValue, isDst);
  }
}
