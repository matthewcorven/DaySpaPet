using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace DaySpaPet.Core.PetAggregate.Specifications;

public class PetById : Specification<Pet>
{
  public PetById(int petId)
  {
    Query.Where(pet => pet.Id == petId);
  }
}
