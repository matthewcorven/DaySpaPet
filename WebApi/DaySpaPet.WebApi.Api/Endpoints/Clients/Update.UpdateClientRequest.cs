using DaySpaPet.WebApi.Core.ClientAggregate;
using System.ComponentModel.DataAnnotations;

namespace DaySpaPet.WebApi.Api.Clients;

public class UpdateClientRequest {
    public const string Route = "/Clients/{ClientId}";
    public static string BuildRoute(int clientId) => Route.Replace("{ClientId}", clientId.ToString());

    public int ClientId { get; set; }

    [Required]
    public int Id { get; set; }
    [Required]
    public string? FirstName { get; init; } = default!;
    [Required]
    public string? LastName { get; init; } = default!;
    [Required]
    public string? PhoneCountryCode { get; init; } = default!;
    [Required]
    public string? PhoneNumber { get; init; } = default!;
    [Required]
    public string Status { get; init; } = ClientAccountStatus.NotSet;
    public string? PhoneExtension { get; init; } = default!;
    public string? EmailAddress { get; init; }
}