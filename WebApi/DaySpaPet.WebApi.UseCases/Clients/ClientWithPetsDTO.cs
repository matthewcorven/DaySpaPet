using System.Collections.ObjectModel;
using DaySpaPet.WebApi.UseCases.Pets;

namespace DaySpaPet.WebApi.UseCases.Clients;

public sealed record ClientWithPetsDTO(int Id, string FirstName, string LastName, 
  string? PhoneCountryCode, string PhoneNumber, string PhoneExtension, 
  string EmailAddress, ReadOnlyCollection<PetDTO> Pets);
