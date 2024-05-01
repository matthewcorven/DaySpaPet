using System.Collections.ObjectModel;

namespace DaySpaPet.WebApi.Api.Clients;

public sealed record ClientListRequest
{
  public const string Route = "/Clients";

  public int? Skip { get; set; }
  public int? Take { get; set; }
}
