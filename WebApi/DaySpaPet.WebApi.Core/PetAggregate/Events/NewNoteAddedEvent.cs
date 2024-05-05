using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.PetAggregate.Events;

public class NewNoteAddedEvent : DomainEventBase
{
  public NewNoteAddedEvent(Pet pet, PetNote newNote, OriginClock originClock) : base(originClock)
  {
    Pet = pet;
    AddedPetNote = newNote;
  }
  public PetNote AddedPetNote { get; private set; }
  public Pet Pet { get; private set; }
}
