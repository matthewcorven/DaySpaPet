using NodaTime;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// An instant of any locale, expressed in semantically-named properties.
/// </summary>
/// <param name="LocalDateTime">Date and time value from the perspective of the locale./param>
/// <param name="TimeZoneId">The IANA (aka TZDB) time zone identifier for the locale.</param>
/// <param name="IsDaylightSavingsTime">If Daylight Savings Time is being observed at the locale in LocalDateTime./param>
public sealed record OriginClock(LocalDateTime LocalDateTime, string TimeZoneId, bool IsDaylightSavingsTime);