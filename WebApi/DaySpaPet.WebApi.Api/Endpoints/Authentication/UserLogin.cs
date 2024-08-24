using DaySpaPet.WebApi.Core.Interfaces;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace DaySpaPet.WebApi.Api.Endpoints.Authentication;

public class UserLogin : Endpoint<LoginRequest> {
  public IAppUserAuthenticationService UserAuthenticationService { get; set; } = default!;

  public override void Configure() {
    Post("/api/authentication/login");
    AllowAnonymous();
  }

  public override async Task HandleAsync(LoginRequest req, CancellationToken ct) {
    var validationResult = await UserAuthenticationService.TryValidateUserCredentialsAsync(req.Email, req.Password, ct);

    if (validationResult.Validated) {
      await SendAsync(null, cancellation: ct);
    } else
      ThrowError("The supplied credentials are invalid!", StatusCodes.Status401Unauthorized);
  }
}