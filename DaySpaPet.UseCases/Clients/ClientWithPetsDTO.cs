using System.Collections.ObjectModel;
using DaySpaPet.UseCases.Pets;

namespace DaySpaPet.UseCases.Clients;

public sealed record ClientWithPetsDTO(int Id, string FirstName, string LastName, 
  string? PhoneCountryCode, string PhoneNumber, string PhoneExtension, 
  string EmailAddress, ReadOnlyCollection<PetDTO> Pets);
