namespace DaySpaPet.WebApi.Api.Clients;

public record DeactivateClientRequest
{
	public const string Route = "/Clients/{ClientId:int}";
	public static string BuildRoute(int contributorId) => Route.Replace("{ClientId:int}", contributorId.ToString());

	public int ClientId { get; set; }
}
