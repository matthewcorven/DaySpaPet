using DaySpaPet.SharedKernel;

namespace DaySpaPet.UseCases.Pets;

public sealed record PetNoteDTO(int Id, int PetId, string Text, CapturedDateTime CreatedAt, 
  bool IsAlert);
