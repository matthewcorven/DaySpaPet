using DaySpaPet.WebApi.Core.Domain.Users;
using DaySpaPet.WebApi.SharedKernel.Extensions;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Serilog;
using DaySpaPet.WebApi.Core.Interfaces;

namespace DaySpaPet.WebApi.Infrastructure.CoreImplementations;
internal class FakeDaySpaUserAuthenticationService : IAppUserAuthenticationService {
  private readonly IConfiguration _config;
  private readonly ILogger _logger;

  public FakeDaySpaUserAuthenticationService(IConfiguration config, Serilog.ILogger logger) {
    _config = config;
    _logger = logger;
  }

  public ValueTask<UserCredentialsValidationResult> TryValidateUserCredentialsAsync(string StatedEmailAddress, string StatedPassword, CancellationToken ct) {
    // Retrieve required configuration values with logging if not found
    IConfigurationSection authSchemeBearer = _config.GetRequiredSection("Authentication:Schemes:Bearer");

    if (!authSchemeBearer.TryGetRequiredConfiguration(_logger, "PublicSigningKey", out string? jwtPublicSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "PrivateSigningKey", out string? jwtPrivateSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidIssuer", out string? jwtIssuer) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidAudiences", out string? jwtAudiences) ||
        !authSchemeBearer.TryGetRequiredConfiguration<int?>(_logger, "TokenExpirationSeconds", out int? tokenExpirationSeconds)) {
      return ValueTask.FromResult(new UserCredentialsValidationResult(false, null));
    }

    List<AssignedUserRolePublicView> userRoles = [];
    List<UserClaimPublicView> userClaims = [];

    userRoles.Add(new AssignedUserRolePublicView("Admin", "Administrator"));
    userRoles.Add(new AssignedUserRolePublicView("Admin", "Administrator"));
    userClaims.Add(new("AdminstratorId", Guid.NewGuid().ToString()));
    userClaims.Add(new("Username", "admin@dayspapet.local"));
    userClaims.Add(new("EmailAddress", "admin@dayspapet.local"));
    userClaims.Add(new("TimeZoneId", "America/New_York"));
    userClaims.Add(new("Locale", "en-US"));
    userClaims.Add(new("Currency", "USD"));
    userClaims.Add(new("FirstName", "Mike"));
    userClaims.Add(new("LastName", "Smith"));
    userClaims.Add(new("MiddleName", "D"));
    userClaims.Add(new("DateOfBirth", new AnnualDate(11, 2).ToString()));
    userClaims.Add(new("ProfileImageUrl", "https://placehold.co/200x200"));
    userClaims.Add(new("PhoneNumber", "+18885551212"));
    userClaims.Add(new("AddressLine1", "123 Main Street"));
    userClaims.Add(new("AddressLine2", "Suite 100"));
    userClaims.Add(new("City", "Hometown"));
    userClaims.Add(new("State", "MI"));
    userClaims.Add(new("PostalCode", "48312"));
    userClaims.Add(new("CountryCode", "USA"));
    
    Instant tokenExpiresAtUtc = Instant.FromDateTimeUtc(DateTime.UtcNow.AddDays(1));

    string jwtToken = JwtBearer.CreateToken(
            o => {
              // Base64 encoded private-key
              o.SigningKey = jwtPrivateSigningKey!;
              o.KeyIsPemEncoded = true;
              o.SigningStyle = TokenSigningStyle.Asymmetric;
              o.SigningAlgorithm = SecurityAlgorithms.RsaSha256;
              o.Issuer = jwtIssuer!;
              o.Audience = jwtAudiences!; // Hmm. Need to set this based on request? Or instead derive audience from user?
              o.ExpireAt = tokenExpiresAtUtc.ToDateTimeUtc();
              foreach (AssignedUserRolePublicView ur in userRoles) {
                o.User.Roles.Add(ur.ShortName);
              }
              foreach (UserClaimPublicView item in userClaims) {
                //o.User.Claims.Add((item.Key, item.Value));
                o.User[item.Key] = item.Value;
              }
            });

    AuthenticatedAppUser appUser = new() {
      Token = jwtToken,
      TokenExpiresAtUtc = tokenExpiresAtUtc,
      Roles = userRoles,
      Claims = userClaims
    };
    return ValueTask.FromResult(new UserCredentialsValidationResult(true, appUser));
  }
}
