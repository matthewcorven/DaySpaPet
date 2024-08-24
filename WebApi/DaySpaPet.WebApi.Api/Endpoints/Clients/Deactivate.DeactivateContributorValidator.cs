using FastEndpoints;
using FluentValidation;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeactivateClientValidator : Validator<DeactivateClientRequest> {
  public DeactivateClientValidator() {
    RuleFor(x => x.ClientId)
            .GreaterThan(0);
  }
}