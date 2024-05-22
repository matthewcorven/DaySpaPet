using DaySpaPet.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class UpdateClientValidator : Validator<UpdateClientRequest>
{
	public UpdateClientValidator()
	{
		RuleFor(x => x.FirstName)
				.NotEmpty()
				.WithMessage("FirstName is required.")
				.MinimumLength(2)
				.MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
		RuleFor(x => x.LastName)
				.NotEmpty()
				.WithMessage("LastName is required.")
				.MinimumLength(2)
				.MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
		RuleFor(x => x.PhoneCountryCode)
				.NotEmpty()
				.WithMessage("Phone Country Code is required.")
				.MinimumLength(2)
				.MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
		RuleFor(x => x.PhoneNumber)
				.NotEmpty()
				.WithMessage("Phone Number is required.")
				.MinimumLength(2)
				.MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
		RuleFor(x => x.PhoneExtension)
				.MinimumLength(1)
				.MaximumLength(10);
		RuleFor(x => x.EmailAddress)
				.EmailAddress()
				.When(x => !string.IsNullOrWhiteSpace(x.EmailAddress))
				.WithMessage("Email Address is not valid.");
		RuleFor(x => x.ClientId)
				.Must((args, clientId) => args.Id == clientId)
				.WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
	}
}
