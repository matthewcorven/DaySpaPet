using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// List Clients
/// </summary>
/// <remarks>
/// List clients - returns a ClientListResponse containing the Clients.
/// </remarks>
public class List
  : Endpoint<ClientListRequest, ClientListResponse>
{
  private readonly IMediator _mediator;

  public List(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get(ClientListRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(ClientListRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(new ListClientsShallowQuery(request.Skip, request.Take), ct);

    if (result.IsSuccess)
    {
      Response = new ClientListResponse
      {
        Clients = result.Value.Select(c => new ClientRecord(c.Id, c.FirstName, c.LastName,
        c.PhoneCountryCode, c.PhoneNumber, c.PhoneExtension!,
        c.Status, c.EmailAddress!)).ToList().AsReadOnly()
      };
    }
  }
}
