using NodaTime;

namespace DaySpaPet.WebApi.Core.Interfaces;

public interface IGlobalizationService {
    public bool TryGetTimeZoneById(string timeZoneId, out DateTimeZone? provider);
}