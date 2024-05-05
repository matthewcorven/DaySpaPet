namespace DaySpaPet.WebApi.Api.Clients;

public class UpdateClientResponse
{
  public UpdateClientResponse(ClientRecord contributor)
  {
    Client = contributor;
  }
  public ClientRecord Client { get; set; }
}
