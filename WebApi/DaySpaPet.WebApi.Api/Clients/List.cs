using Azure;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// List all Clients
/// </summary>
/// <remarks>
/// List all clients - returns a ClientListResponse containing the Clients.
/// NOTE: In DEV always returns a FAKE set of 2 clients
/// </remarks>
public class List
  : EndpointWithoutRequest<ClientListResponse>
{
  private readonly IMediator _mediator;

  public List(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/Clients");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await _mediator.Send(new ListClientsShallowQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new ClientListResponse
      {
        Clients = result.Value.Select(c => new ClientRecord(c.Id, c.FirstName, c.LastName,
        c.PhoneCountryCode, c.PhoneNumber, c.PhoneExtension!,
        c.EmailAddress!)).ToList().AsReadOnly()
      };
    }
  }
}
