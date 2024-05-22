namespace DaySpaPet.WebApi.SharedKernel;

public record AppUserRequestContext
{
	public string UserUPN { get; internal set; } = string.Empty;
	public string[] Roles { get; internal set; } = [];
	public OriginClock ClockSnapshot { get; internal set; } = default!;

	public void Set(string userUPN, string[] roles, OriginClock clockSnapshot)
	{
		UserUPN = userUPN;
		Roles = roles;
		ClockSnapshot = clockSnapshot;
	}
}