using DaySpaPet.WebApi.Core.Domain.Users;

namespace DaySpaPet.WebApi.Core.Interfaces;
public interface IAppUserAuthenticationService {
  ValueTask<(bool Validated, AuthenticatedAppUser? AuthenticatedAppUser)> TryValidateUserCredentialsAsync(string StatedEmailAddress, string StatedPassword, CancellationToken ct);
}
