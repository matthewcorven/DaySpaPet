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

  public ValueTask<bool> TryValidateUserCredentialsAsync(string EmailAddress, string Password, CancellationToken ct, out AuthenticatedAppUser? appUser) {
    appUser = null;

    // Retrieve required configuration values with logging if not found
    IConfigurationSection authSchemeBearer = _config.GetRequiredSection("Authentication:Schemes:Bearer");

    if (!authSchemeBearer.TryGetRequiredConfiguration(_logger, "PublicSigningKey", out string? jwtPublicSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "PrivateSigningKey", out string? jwtPrivateSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidIssuer", out string? jwtIssuer) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidAudiences", out string? jwtAudiences) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "TokenExpirationSeconds", out string? tokenExpirationSeconds)) {
      return ValueTask.FromResult(false);
    }

    string userId = "c3d20521-0f33-491d-8a68-c6bc7a1159d3";
    string adminId = "57a791d6-10e1-4453-a295-f53147959152";
    string[] userRoles = ["Manager", "Administrator"];
    Dictionary<string, string> userClaims = new() {
      { "AdministratorID", adminId },
      { "Username", EmailAddress },
      { "UserID", userId },
    };
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
              foreach (string item in userRoles) {
                o.User.Roles.Add(item);
              }
              foreach (KeyValuePair<string, string> item in userClaims) {
                //o.User.Claims.Add((item.Key, item.Value));
                o.User[item.Key] = item.Value;
              }
            });

    appUser = new AuthenticatedAppUser() {
      Token = jwtToken,
      TokenExpiresAtUtc = tokenExpiresAtUtc,
      Username = EmailAddress,
      EmailAddress = EmailAddress,
      TimeZoneId = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("America/New_York").Id,
      Locale = "en-US",
      Currency = "USD",
      FirstName = "John",
      MiddleName = "Q",
      LastName = "Doe",
      DateOfBirth = new AnnualDate(2000, 15),
      ProfileImageUrl = "https://placehold.co/100x100",
      PhoneNumber = "+15555555555",
      AddressLine1 = "123 Main St",
      AddressLine2 = "Apt 1",
      City = "Anytown",
      State = "NY",
      PostalCode = "12992",
      CountryCode = "USA"
    };

    return ValueTask.FromResult(true);
  }
}
