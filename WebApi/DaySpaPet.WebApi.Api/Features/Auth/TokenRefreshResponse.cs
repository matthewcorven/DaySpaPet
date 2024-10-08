﻿using FastEndpoints.Security;
using System.Globalization;

namespace DaySpaPet.WebApi.Api.Features.Auth;

public class TokenRefreshResponse : TokenResponse {
  //ideally should be using something like nodatime to convert to the local time zone of the client app
  public string AccessTokenExpiry => AccessExpiry.ToLocalTime().ToString(CultureInfo.InvariantCulture);

  public int RefreshTokenValidityMinutes => (int)RefreshExpiry.Subtract(DateTime.UtcNow).TotalMinutes;

  //NOTE: most of the time you will be doing this kind of custom transformation on the expiry datetime properties.
  //      that is why the TokenResponse properties are decorated with [JsonIgnore] attributes.
}