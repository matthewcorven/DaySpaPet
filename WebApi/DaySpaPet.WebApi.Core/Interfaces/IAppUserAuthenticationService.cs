using DaySpaPet.WebApi.Core.AppUserAggregate;
using DaySpaPet.WebApi.Core.Domain.Users;
using NodaTime;

namespace DaySpaPet.WebApi.Core.Interfaces;

public record struct UserCredentialsValidationResult(bool Validated, AuthenticatedAppUser? AuthenticatedAppUser);
public record struct UserRefreshTokenValidationResult(bool Validated, AuthenticatedAppUser? AuthenticatedAppUser);
public interface IAppUserAuthenticationService {
  ValueTask<UserCredentialsValidationResult> TryValidateUserCredentialsAsync(string StatedEmailAddress, string StatedPassword, CancellationToken ct);
  ValueTask<UserRefreshTokenValidationResult> TryValidateUserRefreshTokenAsync(Guid UserId, string RefreshToken, CancellationToken ct);
  Task StoreAppUserRefreshToken(AppUserRefreshToken appUserRefreshToken);
}
