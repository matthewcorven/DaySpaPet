using DaySpaPet.WebApi.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Identity.Data;

namespace DaySpaPet.WebApi.Api.Endpoints.Authentication;

public class UserLogin : Endpoint<LoginRequest> {
  public IAppUserAuthenticationService UserAuthenticationService { get; set; } = default!;

  public override void Configure() {
    Post("/api/authentication/login");
    AllowAnonymous();
  }

  public override async Task HandleAsync(LoginRequest req, CancellationToken ct) {
    UserCredentialsValidationResult validationResult = await UserAuthenticationService.TryValidateUserCredentialsAsync(req.Email, req.Password, ct);

    if (validationResult.Validated && validationResult.AuthenticatedAppUser is not null) {
      await SendAsync(validationResult.AuthenticatedAppUser!.Value.Token, cancellation: ct);
    } else
      ThrowError("The supplied credentials are invalid!", StatusCodes.Status401Unauthorized);
  }
}