using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.PetAggregate.Events;

public class NewNoteAddedEvent(Pet pet, PetNote newNote, OriginClock originClock) 
  : DomainEventBase(originClock)
{
  public PetNote AddedPetNote { get; private set; } = newNote;
  public Pet Pet { get; private set; } = pet;
}
