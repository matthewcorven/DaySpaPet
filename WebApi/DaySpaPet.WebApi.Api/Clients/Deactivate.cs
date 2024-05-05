using FastEndpoints;
using Ardalis.Result;
using MediatR;
using DaySpaPet.WebApi.UseCases.Clients.Deactivate;
using DaySpaPet.WebApi.Core.Interfaces;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// Delete a Client.
/// </summary>
/// <remarks>
/// Delete a Client by providing a valid integer id.
/// </remarks>
public class Deactivate : Endpoint<DeactivateClientRequest>
{
  private readonly IMediator _mediator;

  public Deactivate(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete(DeactivateClientRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    DeactivateClientRequest request,
    CancellationToken cancellationToken)
  {
    IRequestOriginContextProvider rocP = Resolve<IRequestOriginContextProvider>();
    var command = new DeactivateClientCommand(request.ClientId, rocP.GetOriginClock());

    var result = await _mediator.Send(command);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    };
    // TODO: Handle other issues as needed
  }
}
