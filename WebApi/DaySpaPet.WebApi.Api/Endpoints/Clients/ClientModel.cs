namespace DaySpaPet.WebApi.Api.Endpoints.Clients;

public sealed record ClientModel(int Id, string FirstName, string LastName,
        string? PhoneCountryCode, string PhoneNumber, string PhoneExtension,
        string Status, string EmailAddress);