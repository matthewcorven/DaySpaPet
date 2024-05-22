namespace DaySpaPet.WebApi.Api.Clients;

public class UpdateClientResponse
{
	public UpdateClientResponse(ClientRecord client)
	{
		Client = client;
	}
	public ClientRecord Client { get; set; }
}
