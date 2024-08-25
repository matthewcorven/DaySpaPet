using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.PetAggregate.Events;

public record NewNoteAddedEvent(
  int ClientId,
  int PetId,
  string PetName,
  AnimalType PetAnimalType,
  string PetBreed,
  PetStatus PetStatus,
  string PetNoteText,
  bool PetNoteIsAlert,
  OriginClock OriginClock) : DomainEventBase(OriginClock);