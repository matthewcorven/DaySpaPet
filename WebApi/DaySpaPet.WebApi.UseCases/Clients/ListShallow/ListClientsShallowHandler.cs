using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.ListShallow;

public sealed record ListClientsShallowHandler(IListClientsShallowQueryService query)
		: IQueryHandler<ListClientsShallowQuery, Result<IEnumerable<ClientDTO>>>
{
	private readonly IListClientsShallowQueryService _query = query;

	public async Task<Result<IEnumerable<ClientDTO>>> Handle(
			ListClientsShallowQuery request, CancellationToken cancellationToken)
	{
		var result = await _query.ListAsync(request.Skip, request.Take);

		return Result.Success(result);
	}
}
