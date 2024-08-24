using Ardalis.Result;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.Deactivate;

public class DeactivateClientHandler : ICommandHandler<DeactivateClientCommand, Result> {
  private readonly IDeactivateClientService _deleteClientService;

  public DeactivateClientHandler(IDeactivateClientService deleteClientService) {
    _deleteClientService = deleteClientService;
  }

  public async Task<Result> Handle(DeactivateClientCommand request, CancellationToken cancellationToken) {
    // This Approach: Keep Domain Events in the Domain Model / Core project; this becomes a pass-through
    return await _deleteClientService.DeactivateClient(request.ClientId, request.originClock);

    // Another Approach: Do the real work here including dispatching domain events - change the event from internal to public
    // Ardalis prefers using the service so that "domain event" behavior remains in the domain model / core project
    // var aggregateToDelete = await _repository.GetByIdAsync(request.ClientId);
    // if (aggregateToDelete == null) return Result.NotFound();

    // await _repository.DeleteAsync(aggregateToDelete);
    // var domainEvent = new ClientDeletedEvent(request.ClientId);
    // await _mediator.Publish(domainEvent);
    // return Result.Success();
  }
}