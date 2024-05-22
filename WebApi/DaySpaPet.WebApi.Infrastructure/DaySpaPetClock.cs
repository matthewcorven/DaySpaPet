using NodaTime;

namespace DaySpaPet.WebApi.Infrastructure;

public class DaySpaPetClock : IClock
{
	public Instant GetCurrentInstant()
	{
		return SystemClock.Instance.GetCurrentInstant();
	}
}
