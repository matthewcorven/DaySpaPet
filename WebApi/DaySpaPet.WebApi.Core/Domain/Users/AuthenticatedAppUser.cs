using NodaTime;

namespace DaySpaPet.WebApi.Core.Domain.Users;

public readonly record struct AssignedUserRolePublicView(string ShortName, string LongName);
public readonly record struct UserClaimPublicView(string Key, string Value);

public readonly record struct AuthenticatedAppUser {
  public required string Token { get; init; }
  public required Instant TokenExpiresAtUtc { get; init; }
  public required List<AssignedUserRolePublicView> Roles { get; init; }
  public required List<UserClaimPublicView> Claims { get; init; }
}