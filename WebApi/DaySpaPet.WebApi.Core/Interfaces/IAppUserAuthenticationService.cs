using DaySpaPet.WebApi.Core.Domain.Users;

namespace DaySpaPet.WebApi.Core.Interfaces;

public record struct UserCredentialsValidationResult(bool Validated, AuthenticatedAppUser? AuthenticatedAppUser);
public interface IAppUserAuthenticationService {
  ValueTask<UserCredentialsValidationResult> TryValidateUserCredentialsAsync(string StatedEmailAddress, string StatedPassword, CancellationToken ct);
}
