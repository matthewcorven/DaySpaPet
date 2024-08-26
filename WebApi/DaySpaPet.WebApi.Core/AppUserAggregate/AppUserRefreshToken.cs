using Ardalis.GuardClauses;
using DaySpaPet.WebApi.SharedKernel;
using NodaTime;
using System.Diagnostics;

namespace DaySpaPet.WebApi.Core.AppUserAggregate;
public class AppUserRefreshToken : EntityBase<Guid> {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public AppUserRefreshToken()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public AppUserRefreshToken(Guid appUserId, DateTime refreshExpiry, string refreshToken, OriginClock originClock) {
    AppUserId = Guard.Against.NullOrEmpty(appUserId, nameof(appUserId));
    Guard.Against.Default(refreshExpiry, nameof(refreshExpiry));
    if (refreshExpiry.Kind != DateTimeKind.Utc) {
      Debugger.Break();
      throw new ArgumentException("Refresh expiry must be in UTC", nameof(refreshExpiry));
    }
    RefreshExpiry = Instant.FromDateTimeUtc(refreshExpiry);
    RefreshToken = Guard.Against.NullOrEmpty(refreshToken, nameof(refreshToken));

    SetCreatedAt(originClock);
  }

  public Guid AppUserId { get; private set; }
  public Instant RefreshExpiry { get; private set; }
  public string RefreshToken { get; private set; }
}
