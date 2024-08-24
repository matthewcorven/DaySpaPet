using Ardalis.Result;
using DaySpaPet.WebApi.Api.Clients;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Clients.Deactivate;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Endpoints.Clients;

/// <summary>
/// Delete a Client.
/// </summary>
/// <remarks>
/// Delete a Client by providing a valid integer id.
/// </remarks>
public class Deactivate : Endpoint<DeactivateClientRequest> {
    private readonly IMediator _mediator;
    private readonly AppUserRequestContext _appUserRequestContext;

    public Deactivate(IMediator mediator, AppUserRequestContext appUserRequestContext) {
        _mediator = mediator;
        _appUserRequestContext = appUserRequestContext;
    }

    public override void Configure() {
        Delete(DeactivateClientRequest.Route);
        AllowAnonymous();
    }

    public override async Task HandleAsync(
            DeactivateClientRequest request,
            CancellationToken cancellationToken) {
        var command = new DeactivateClientCommand(
                request.ClientId, _appUserRequestContext.ClockSnapshot);

        var result = await _mediator.Send(command);

        if (result.Status == ResultStatus.NotFound) {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        if (result.IsSuccess) {
            await SendNoContentAsync(cancellationToken);
        };
        // TODO: Handle other issues as needed
    }
}