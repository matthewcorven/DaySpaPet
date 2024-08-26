using DaySpaPet.WebApi.Core.AppUserAggregate;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.SharedKernel.Extensions;
using FastEndpoints;
using FastEndpoints.Security;
using System.Security.Claims;

namespace DaySpaPet.WebApi.Api.Features.Auth.RefreshToken;

public class AppUserRefreshTokenService : RefreshTokenService<TokenRequest, TokenRefreshResponse> {
  private readonly Serilog.ILogger _logger;
  private readonly IAppUserAuthenticationService _userAuthenticationService;
  private readonly AppUserRequestContext _appUserRequestContext;

  public AppUserRefreshTokenService(
    IConfiguration config, 
    Serilog.ILogger logger,
    IAppUserAuthenticationService userAuthenticationService, 
    AppUserRequestContext appUserRequestContext) {
    
    _logger = logger;
    _userAuthenticationService = userAuthenticationService;
    _appUserRequestContext = appUserRequestContext;

    IConfigurationSection authSchemeBearer = config.GetRequiredSection("Authentication:Schemes:Bearer");
    if (!authSchemeBearer.TryGetRequiredConfiguration(_logger, "PublicSigningKey", out string? jwtPublicSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "PrivateSigningKey", out string? jwtPrivateSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidIssuer", out string? jwtIssuer) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidAudiences", out string? jwtAudiences) ||
        !authSchemeBearer.TryGetRequiredConfiguration<int?>(_logger, "TokenExpirationSeconds", out int? tokenExpirationSeconds)) {
      return;
    }

    Setup(x => {
      x.TokenSigningKey = jwtPrivateSigningKey;
      x.AccessTokenValidity = TimeSpan.FromMinutes(1);
      x.RefreshTokenValidity = TimeSpan.FromHours(1);
      x.Endpoint("/user/authentication/refresh-token", ep => {
        ep.Summary(s => s.Description = "this is the refresh token endpoint");
      });
    });
  }

  public override async Task PersistTokenAsync(TokenRefreshResponse rsp) {
    if (!Guid.TryParse(rsp.UserId, out Guid userId))
      throw new("The user id is not valid!");
    await _userAuthenticationService.StoreAppUserRefreshToken(new AppUserRefreshToken(userId, rsp.RefreshExpiry, rsp.RefreshToken, _appUserRequestContext.ClockSnapshot));
  }

  public override async Task RefreshRequestValidationAsync(TokenRequest req) {
    if (string.IsNullOrWhiteSpace(req.RefreshToken))
      AddError("The refresh token is required!");
    if (!Guid.TryParse(req.UserId, out Guid userId))
      AddError("The user id is not valid!");

    UserRefreshTokenValidationResult result = 
      await _userAuthenticationService.TryValidateUserRefreshTokenAsync(userId, req.RefreshToken, CancellationToken.None);
    if (result.Validated is false)
      AddError("The refresh token is not valid!");
  }

  public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges) {
    if (!Guid.TryParse(request.UserId, out Guid userId))
      throw new("The user id is not valid!");
    UserRefreshTokenValidationResult validationResult = await _userAuthenticationService.TryValidateUserRefreshTokenAsync(userId, request.RefreshToken, CancellationToken.None);

    if (validationResult.Validated && validationResult.AuthenticatedAppUser is not null) {
      privileges.Roles.AddRange(validationResult.AuthenticatedAppUser.Value.Roles.Select(r => r.ShortName));
      privileges.Claims.AddRange(validationResult.AuthenticatedAppUser.Value.Claims.Select(c => new Claim(c.Key, c.Value)));
    }
  }
}