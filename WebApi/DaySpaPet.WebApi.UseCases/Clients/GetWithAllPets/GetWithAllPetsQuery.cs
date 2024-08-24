using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.GetWithAllPets;

public sealed record GetWithAllPetsQuery(int ClientId) : IQuery<Result<ClientWithPetsDTO>>;