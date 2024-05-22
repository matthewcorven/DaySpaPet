using System.ComponentModel.DataAnnotations;

namespace DaySpaPet.WebApi.Api.Clients;

public record CreateClientRequest
{
	public const string Route = "/Clients";

	[Required]
	public string? FirstName { get; init; }
	[Required]
	public string? LastName { get; init; }
	[Required]
	public string? PhoneCountryCode { get; init; }
	[Required]
	public string? PhoneNumber { get; init; }
	public string? PhoneExtension { get; init; }
	public string? EmailAddress { get; init; }
}
