using Ardalis.GuardClauses;
using System.Runtime.CompilerServices;

namespace DaySpaPet.WebApi.SharedKernel.GuardClauses;

public static class StringGuardExtensions {
  /// <summary>
  /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is not a valid email
  /// address. Validation performed by .NET built-in
  /// <see cref="System.ComponentModel.DataAnnotations.EmailAddressAttribute"/>
  /// as well as adherence to the length restrictions as defined in RFC 3696.
  /// </summary>
  /// <param name="guardClause"></param>
  /// <param name="input"></param>
  /// <param name="inputParameterName"></param>
  /// <param name="message">Optional. Custom error message</param>
  /// <returns><paramref name="input" /> if the value is before <paramref name="compareToDate" />.</returns>
  /// <exception cref="ArgumentException"></exception>
  /// <ref>https://www.rfc-editor.org/rfc/rfc3696#section-3</ref>
  public static string EmailInvalid(this IGuardClause guardClause,
                  string input,
                  [CallerArgumentExpression(nameof(input))] string? inputParameterName = null,
                  string? message = null) {
    if (string.IsNullOrEmpty(input)) {
      return input;
    }

    System.ComponentModel.DataAnnotations.EmailAddressAttribute ea = new();
    if (!ea.IsValid(input.Trim())) {
      throw new ArgumentException(message ?? $"Invalid email address \"{input}\".", inputParameterName);
    }

    string[] parts = input.Split('@');
    if (input.Length > 320 || parts.Length != 2 || parts[0].Length > 64 || parts[1].Length > 255) {
      throw new ArgumentException(message ?? $"Invalid email address \"{input}\". Length of local or domain name, or overall length, exceeds standards.", inputParameterName);
    }

    return input;
  }
}