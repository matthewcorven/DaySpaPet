namespace DaySpaPet.WebApi.SharedKernel;

public record AppUserRequestContext {
  public string Username { get; internal set; } = string.Empty;
  public string[] Roles { get; internal set; } = [];
  public OriginClock ClockSnapshot { get; internal set; } = default!;

  public void Set(string username, string[] roles, OriginClock clockSnapshot) {
    Username = username;
    Roles = roles;
    ClockSnapshot = clockSnapshot;
  }
}