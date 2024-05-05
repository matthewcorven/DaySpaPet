namespace DaySpaPet.WebApi.Api.Clients;

public sealed record ClientRecord(int Id, string FirstName, string LastName, 
  string? PhoneCountryCode, string PhoneNumber, string PhoneExtension, 
  string Status, string EmailAddress);
