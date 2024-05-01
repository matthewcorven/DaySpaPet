using NodaTime;

namespace DaySpaPet.WebApi.Core.Interfaces;

public sealed record OriginClock(LocalDateTime LocalDateTime, string TimeZoneId, bool IsDaylightSavingsTime);
