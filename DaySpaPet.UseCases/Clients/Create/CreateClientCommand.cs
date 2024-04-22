using Ardalis.Result;
using Ardalis.SharedKernel;
using DaySpaPet.SharedKernel;

namespace DaySpaPet.UseCases.Clients.Create;

/// <summary>
/// Create a new Client.
/// </summary>
/// <param name="firstName">Required.</param>
/// <param name="lastName">Required.</param>
/// <param name="phoneCountryCode">Optional; Defaults to <see cref="DaySpaPet.Core.Defaults.DefaultPhoneCountryCode"/>.</param>
/// <param name="phoneNumber">Required</param>
/// <param name="phoneExtension">Optional; Defaults to null.</param>
/// <param name="emailAddress">Required. Validated based on format and length.</param>
public record CreateClientCommand(string firstName, string lastName, string? phoneCountryCode, 
  string phoneNumber, string? phoneExtension, string emailAddress,
  CapturedDateTime createdAt) : ICommand<Result<int>>;
