using DaySpaPet.WebApi.Core.Interfaces;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity.Data;
using System.Globalization;

namespace DaySpaPet.WebApi.Api.Endpoints.Authentication;

public class UserLogin : Endpoint<LoginRequest, MyTokenResponse> {
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

public class MyTokenResponse : TokenResponse {
  //ideally should be using something like nodatime to convert to the local time zone of the client app
  public string AccessTokenExpiry => AccessExpiry.ToLocalTime().ToString(CultureInfo.InvariantCulture);

  public int RefreshTokenValidityMinutes => (int)RefreshExpiry.Subtract(DateTime.UtcNow).TotalMinutes;

  //NOTE: most of the time you will be doing this kind of custom transformation on the expiry datetime properties.
  //      that is why the TokenResponse properties are decorated with [JsonIgnore] attributes.
}