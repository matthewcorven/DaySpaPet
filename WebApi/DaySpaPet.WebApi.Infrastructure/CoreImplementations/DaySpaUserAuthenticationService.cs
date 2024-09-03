
using DaySpaPet.WebApi.Core.Domain.Users;
using DaySpaPet.WebApi.SharedKernel.Extensions;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Serilog;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using DaySpaPet.WebApi.Core.AppUserAggregate;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.WebApi.Infrastructure.CoreImplementations;

internal class DaySpaUserAuthenticationService : IAppUserAuthenticationService {
  private readonly IConfiguration _config;
  private readonly ILogger _logger;
  private readonly AppDbContext _appDbContext;

  public DaySpaUserAuthenticationService(IServiceScopeFactory serviceScopeFactory) {
    using IServiceScope scope = serviceScopeFactory.CreateScope();
    IServiceProvider serviceProvider = scope.ServiceProvider;

    _config = serviceProvider.GetRequiredService<IConfiguration>();
    _logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
    _appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
  }

  public async ValueTask<UserCredentialsValidationResult> TryValidateUserCredentialsAsync(string StatedEmailAddress, string StatedPassword, CancellationToken ct) {
    // Step 1: Retrieve the user from the database based on the stated email address
    AppUser? dbAppUser = await _appDbContext.AppUsers
      .SingleOrDefaultAsync(au => au.EmailAddress.Equals(StatedEmailAddress), cancellationToken: ct);
    if (dbAppUser is null) {
      // TODO: Logging?
      return new UserCredentialsValidationResult(false, null);
    }

    // Step 2: Compute the hash using C# code based on retrieved User hashing algorithm and salt
    string computedHash = ComputeHash(dbAppUser.HashingAlgorithm, StatedPassword, dbAppUser.PasswordSalt);

    // Step 3: Compare the computed hash with the stored hash
    if (!computedHash.Equals(dbAppUser.PasswordHash)) {
      // Password is invalid!
      return new UserCredentialsValidationResult(false, null);
    }

    AuthenticatedAppUser? authenticatedAppUser = GetAuthenticatedAppUser(dbAppUser!);
    return new UserCredentialsValidationResult(authenticatedAppUser is not null, authenticatedAppUser);
  }

  public async ValueTask<UserRefreshTokenValidationResult> TryValidateUserRefreshTokenAsync(Guid UserId, string RefreshToken, CancellationToken ct) {
    AppUserRefreshToken? token = await _appDbContext.AppUserRefreshTokens.SingleOrDefaultAsync(x =>
      x.Id == UserId
      && x.RefreshToken == RefreshToken
      && x.RefreshExpiry.ToDateTimeUtc() >= DateTime.UtcNow, cancellationToken: ct);
    if (token is null) {
      return new UserRefreshTokenValidationResult(false, null);
    }

    AppUser? dbAppUser = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Id == UserId, cancellationToken: ct);
    if (dbAppUser is null) {
      return new UserRefreshTokenValidationResult(false, null);
    }

    AuthenticatedAppUser? authenticatedAppUser = GetAuthenticatedAppUser(dbAppUser);
    if (authenticatedAppUser is null) {
      return new UserRefreshTokenValidationResult(false, null);
    }

    return new UserRefreshTokenValidationResult(true, authenticatedAppUser);
  }

  public async Task StoreAppUserRefreshToken(AppUserRefreshToken appUserRefreshToken) {
    _appDbContext.AppUserRefreshTokens.Add(appUserRefreshToken);
    await _appDbContext.SaveChangesAsync();
  }

  private static string ComputeHash(string hashingAlgorithmName, string password, string salt) {

    HashAlgorithm hashAlgorithm = hashingAlgorithmName switch {
      "SHA256" => SHA256.Create(),
      "SHA512" => SHA512.Create(),
      _ => throw new NotSupportedException($"Unsupported hashing algorithm: {hashingAlgorithmName}; must be one of SHA256, SHA512")
    };

    // Combine password and salt
    byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);

    // Compute the hash
    byte[] hashBytes = hashAlgorithm.ComputeHash(combinedBytes);

    // Convert to a base64 string or hex string as needed
    return Convert.ToBase64String(hashBytes);
  }

  private AuthenticatedAppUser? GetAuthenticatedAppUser(AppUser dbAppUser) {
    IConfigurationSection authSchemeBearer = _config.GetRequiredSection("Authentication:Schemes:Bearer");
    if (!authSchemeBearer.TryGetRequiredConfiguration(_logger, "PublicSigningKey", out string? jwtPublicSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "PrivateSigningKey", out string? jwtPrivateSigningKey) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidIssuer", out string? jwtIssuer) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "ValidAudiences", out string? jwtAudiences) ||
        !authSchemeBearer.TryGetRequiredConfiguration(_logger, "TokenExpirationSeconds", out string? tokenExpirationSeconds)) {
      return null;
    }

    List<AssignedUserRolePublicView> userRoles
      = [.. (from aur in _appDbContext.AppUserAssignedRoles
                          join ar in _appDbContext.AppUserRoles on aur.AppUserRoleId equals ar.Id
                          where aur.AppUserId == dbAppUser.Id
          select new AssignedUserRolePublicView(ar.ShortName, ar.LongName))];
    List<UserClaimPublicView> userClaims = [];

    if (dbAppUser.AdministratorId.HasValue) {
      userRoles.Add(new AssignedUserRolePublicView("Admin", "Administrator"));
      userClaims.Add(new UserClaimPublicView("AdministratorId", dbAppUser.AdministratorId.ToString()!));
    }

    // Set all other token properties of AppUser to userClaims, being careful not to leak anything which
    // must remain private for security purposes. Be verbose rather than using reflection or other
    // trickery...
    //
    // Let's carefully and explicitly declare every property which we'll set into the client token.
    userClaims.Add(new("UserID", dbAppUser.Id.ToString()!));
    userClaims.Add(new("Username", dbAppUser.Username));
    userClaims.Add(new("EmailAddress", dbAppUser.EmailAddress));
    userClaims.Add(new("TimeZoneId", dbAppUser.TimeZoneId));
    userClaims.Add(new("Locale", dbAppUser.Locale));
    userClaims.Add(new("Currency", dbAppUser.Currency));
    userClaims.Add(new("FirstName", dbAppUser.FirstName));
    userClaims.Add(new("LastName", dbAppUser.LastName));
    if (dbAppUser.MiddleName is not null) {
      userClaims.Add(new("MiddleName", dbAppUser.MiddleName));
    }
    if (dbAppUser.DateOfBirth is not null) {
      userClaims.Add(new("DateOfBirth", dbAppUser.DateOfBirth.ToString()!));
    }
    if (dbAppUser.ProfileImageUrl is not null) {
      userClaims.Add(new("ProfileImageUrl", dbAppUser.ProfileImageUrl));
    }
    if (dbAppUser.PhoneNumber is not null) {
      userClaims.Add(new("PhoneNumber", dbAppUser.PhoneNumber));
    }
    if (dbAppUser.AddressLine1 is not null) {
      userClaims.Add(new("AddressLine1", dbAppUser.AddressLine1));
    }
    if (dbAppUser.AddressLine2 is not null) {
      userClaims.Add(new("AddressLine2", dbAppUser.AddressLine2));
    }
    if (dbAppUser.City is not null) {
      userClaims.Add(new("City", dbAppUser.City));
    }
    if (dbAppUser.State is not null) {
      userClaims.Add(new("State", dbAppUser.State));
    }
    if (dbAppUser.PostalCode is not null) {
      userClaims.Add(new("PostalCode", dbAppUser.PostalCode));
    }
    if (dbAppUser.CountryCode is not null) {
      userClaims.Add(new("CountryCode", dbAppUser.CountryCode));
    }

    Instant tokenExpiresAtUtc = Instant.FromDateTimeUtc(DateTime.UtcNow.AddSeconds(int.Parse(tokenExpirationSeconds!)));

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

    AuthenticatedAppUser authenticatedAppUser = new() {
      Token = jwtToken,
      TokenExpiresAtUtc = tokenExpiresAtUtc,
      Roles = userRoles,
      Claims = userClaims
    };

    return authenticatedAppUser;
  }
}
