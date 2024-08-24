using DaySpaPet.WebApi.Api.Endpoints.Clients;

namespace DaySpaPet.WebApi.Api.Clients;

public class UpdateClientResponse {
  public UpdateClientResponse(ClientModel client) {
    Client = client;
  }
  public ClientModel Client { get; set; }
}