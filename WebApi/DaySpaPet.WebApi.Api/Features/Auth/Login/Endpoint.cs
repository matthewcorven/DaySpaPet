using DaySpaPet.WebApi.Api.Features.Auth.RefreshToken;
using DaySpaPet.WebApi.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace DaySpaPet.WebApi.Api.Features.Auth.Login;

public class UserLogin : Endpoint<LoginRequest, TokenRefreshResponse> {
  public IAppUserAuthenticationService UserAuthenticationService { get; set; } = default!;

  public override void Configure() {
    Post("/api/authentication/login");
    AllowAnonymous();
  }

  public override async Task HandleAsync(LoginRequest req, CancellationToken ct) {
    UserCredentialsValidationResult validationResult = await UserAuthenticationService.TryValidateUserCredentialsAsync(req.Email, req.Password, ct);

    if (validationResult.Validated && validationResult.AuthenticatedAppUser is not null) {
      string userID = validationResult.AuthenticatedAppUser.Value.Claims
        .SingleOrDefault(c => c.Key == "UserID").Value 
        ?? throw new("UserID claim not found!");
      Response = await CreateTokenWith<UserTokenService>(userID, p => {
        p.Roles.AddRange(validationResult.AuthenticatedAppUser.Value.Roles.Select(r => r.ShortName));
        p.Claims.AddRange(validationResult.AuthenticatedAppUser.Value.Claims.Select(c => new Claim(c.Key, c.Value)));
      });
    } else
      ThrowError("The supplied credentials are invalid!", StatusCodes.Status401Unauthorized);
  }
}