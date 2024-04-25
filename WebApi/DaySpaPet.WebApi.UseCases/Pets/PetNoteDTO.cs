using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Pets;

public sealed record PetNoteDTO(int Id, int PetId, string Text, CapturedDateTime CreatedAt, 
  bool IsAlert);
