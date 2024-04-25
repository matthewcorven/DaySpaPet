using System.Collections.ObjectModel;

namespace DaySpaPet.WebApi.Api.Clients;

public class ClientListResponse
{
  public required ReadOnlyCollection<ClientRecord> Clients { get; set; }
}
