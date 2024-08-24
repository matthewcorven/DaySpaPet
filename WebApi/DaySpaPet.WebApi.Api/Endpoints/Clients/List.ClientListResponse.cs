using DaySpaPet.WebApi.Api.Endpoints.Clients;
using System.Collections.ObjectModel;

namespace DaySpaPet.WebApi.Api.Clients;

public class ClientListResponse {
  public required ReadOnlyCollection<ClientModel> Clients { get; set; }
}