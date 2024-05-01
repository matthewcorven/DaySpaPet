using System.ComponentModel.DataAnnotations;

namespace DaySpaPet.WebApi.Api.Clients;

public class CreateClientRequest
{
  public const string Route = "/Clients";

  [Required]
  public string? FirstName { get; private set; }
  [Required]
  public string? LastName { get; private set; }
  [Required]
  public string? PhoneCountryCode { get; private set; }
  [Required]
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? EmailAddress { get; private set; }
}
