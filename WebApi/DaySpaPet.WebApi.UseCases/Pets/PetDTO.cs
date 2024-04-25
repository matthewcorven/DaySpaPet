using DaySpaPet.WebApi.Core;

namespace DaySpaPet.WebApi.UseCases.Pets;

public sealed record PetDTO(int Id, int ClientId, string Name, AnimalType Type, string Breed, 
  double? Weight, string Status, int? Age, 
  DateOnly? BirthDate, DateOnly? AdoptionDate, DateOnly? DeathDate, 
  DateOnly? MostRecentVisitDate, DateOnly? FirstVisitDate,
  IEnumerable<PetNoteDTO> Notes);
