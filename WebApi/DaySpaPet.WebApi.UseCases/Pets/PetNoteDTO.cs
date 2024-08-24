using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Pets;

public sealed record PetNoteDTO(int Id, int PetId, string Text, bool IsAlert);