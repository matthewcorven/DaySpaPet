using Ardalis.Result;
using Ardalis.SharedKernel;

namespace DaySpaPet.UseCases.Clients.GetWithAllPets;

public sealed record GetWithAllPetsQuery(int ClientId) : IQuery<Result<ClientWithPetsDTO>>;
