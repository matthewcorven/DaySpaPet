using Ardalis.Specification;

namespace DaySpaPet.WebApi.Core.PetAggregate.Specifications;

public class PetById : Specification<Pet>
{
  public PetById(int petId)
  {
    Query.Where(pet => pet.Id == petId);
  }
}
