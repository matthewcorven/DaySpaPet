using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.PetAggregate.Events;

public class NewNoteAddedEvent(Pet pet, PetNote newNote) : DomainEventBase
{
  public PetNote AddedPetNote { get; set; } = newNote;
  public Pet Pet { get; set; } = pet;
}
