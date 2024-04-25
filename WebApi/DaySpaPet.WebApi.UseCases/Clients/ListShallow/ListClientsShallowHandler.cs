using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.SharedKernel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DaySpaPet.WebApi.UseCases.Clients.ListShallow;

public sealed record ListClientsShallowHandler(IListClientsShallowQueryService query)
  : IQueryHandler<ListClientsShallowQuery, Result<IEnumerable<ClientDTO>>>
{
  private readonly IListClientsShallowQueryService _query = query;

  public async Task<Result<IEnumerable<ClientDTO>>> Handle(ListClientsShallowQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();

    return Result.Success(result);
  }
}
