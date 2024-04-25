using Ardalis.Result;
using Ardalis.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.ListShallow;

public sealed record ListClientsShallowQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<ClientDTO>>>;
