using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.Update;

/// <summary>
/// Create a new Client.
/// </summary>
/// <param name="ClientId">Required.</param>
/// <param name="FirstName">Required.</param>
/// <param name="LastName">Required.</param>
/// <param name="PhoneCountryCode">Optional; Defaults to <see cref="DaySpaPet.WebApi.Core.Defaults.DefaultPhoneCountryCode"/>.</param>
/// <param name="PhoneNumber">Required</param>
/// <param name="PhoneExtension">Optional; Defaults to null.</param>
/// <param name="EmailAddress">Required. Validated based on format and length.</param>
/// <param name="OriginClock">Required. An instant of any locale, expressed in semantically-named properties.</param>
public record UpdateClientCommand(int ClientId, string FirstName, string LastName, 
  string? PhoneCountryCode, string PhoneNumber, string PhoneExtension, 
  string Status, string EmailAddress, OriginClock OriginClock) : ICommand<Result<ClientDTO>>;
