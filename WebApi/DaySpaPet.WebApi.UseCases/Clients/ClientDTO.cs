namespace DaySpaPet.WebApi.UseCases.Clients;

public sealed record ClientDTO(int Id, string FirstName, string LastName, 
  string? PhoneCountryCode, string PhoneNumber, string PhoneExtension, 
  string Status, string EmailAddress);
