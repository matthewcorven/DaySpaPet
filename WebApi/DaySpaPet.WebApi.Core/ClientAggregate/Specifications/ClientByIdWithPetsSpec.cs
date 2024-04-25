using Ardalis.Specification;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Specifications;

public sealed class ClientByIdWithPetsSpec : Specification<Client>
{
  public ClientByIdWithPetsSpec(int clientId)
  {
    Query.Where(client => client.Id == clientId)
      .Include(client => client.Pets);
  }
}
