using DaySpaPet.WebApi.UseCases.Clients;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Microsoft.EntityFrameworkCore;

namespace DaySpaPet.WebApi.Infrastructure.Data.Queries;
public class ListClientsShallowQueryService
  : IListClientsShallowQueryService
{
  // You can use EF, Dapper, SqlClient, etc. for queries
  private readonly AppDbContext _db;

  public ListClientsShallowQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<IEnumerable<ClientDTO>> ListAsync()
  {
    var result = await _db.Clients.FromSqlRaw("""
SELECT 
  Id
  ,FirstName
  ,LastName
  ,PhoneCountryCode
  ,PhoneNumber
  ,PhoneExtension
  ,EmailAddress
FROM Clients
""") // don't fetch other big columns
      .Select(c => new ClientDTO(c.Id, c.FirstName, c.LastName, 
        c.PhoneCountryCode, c.PhoneNumber, c.PhoneExtension!,
        c.EmailAddress!))
      .ToListAsync();

    return result;
  }
}
