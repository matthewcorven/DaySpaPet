using DaySpaPet.WebApi.Core.AppUserAggregate;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.SharedKernel;
using FastEndpoints;
using FastEndpoints.Security;
using NodaTime;
using NodaTime.Extensions;
using System.Security.Claims;

namespace DaySpaPet.WebApi.Api.Features.Auth.RefreshToken;

public class UserTokenService : RefreshTokenService<TokenRequest, TokenRefreshResponse> {
  private readonly AppDbContext _dbContext;
  private readonly IClock _clock;
  private readonly IAppUserAuthenticationService _userAuthenticationService;

  public UserTokenService(IConfiguration config, AppDbContext dbContext, IClock clock, IAppUserAuthenticationService userAuthenticationService) {
    _dbContext = dbContext;
    _clock = clock;
    _userAuthenticationService = userAuthenticationService;
    Setup(x => {
      x.TokenSigningKey = config["JWTSigningKey"];
      x.AccessTokenValidity = TimeSpan.FromMinutes(1);
      x.RefreshTokenValidity = TimeSpan.FromHours(1);
      x.Endpoint("/user/auth/refresh-token", ep => {
        ep.Summary(s => s.Description = "this is the refresh token endpoint");
      });
    });
  }

  public override Task PersistTokenAsync(TokenRefreshResponse rsp) {
    if (!Guid.TryParse(rsp.UserId, out Guid userId))
      throw new("The user id is not valid!");
    OriginClock cc = new(DateTime.Now.ToLocalDateTime(), DateTimeZoneProviders.Tzdb.GetSystemDefault().Id, true);
    return _dbContext.AppUserRefreshTokens.AddAsync(new AppUserRefreshToken(userId, rsp.RefreshExpiry, rsp.RefreshToken, OriginClock.Instance));
  }

  public override Task RefreshRequestValidationAsync(TokenRequest req) {
    if (string.IsNullOrWhiteSpace(req.RefreshToken))
      AddError("The refresh token is required!");
    if (!Guid.TryParse(req.UserId, out Guid userId))
      AddError("The user id is not valid!");

    AppUserRefreshToken? token = _dbContext.AppUserRefreshTokens.SingleOrDefault(x =>
      x.Id == userId
      && x.RefreshToken == req.RefreshToken
      && x.RefreshExpiry.ToDateTimeUtc() >= DateTime.UtcNow);
    if (token is null)
      AddError("The refresh token is not valid!");

    return Task.CompletedTask;
  }

  public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges) {
    if (!Guid.TryParse(request.UserId, out Guid userId))
      throw new("The user id is not valid!");
    UserRefreshTokenValidationResult validationResult = await _userAuthenticationService.TryValidateUserRefreshTokenAsync(userId, request.RefreshToken, CancellationToken.None);

    if (validationResult.Validated && validationResult.AuthenticatedAppUser is not null) {
      string userID = validationResult.AuthenticatedAppUser.Value.Claims
        .SingleOrDefault(c => c.Key == "UserID").Value
        ?? throw new("UserID claim not found!");
      privileges.Claims.Add(new("UserID", request.UserId));
      privileges.Roles.AddRange(validationResult.AuthenticatedAppUser.Value.Roles.Select(r => r.ShortName));
      privileges.Claims.AddRange(validationResult.AuthenticatedAppUser.Value.Claims.Select(c => new Claim(c.Key, c.Value)));
    }
  }
}