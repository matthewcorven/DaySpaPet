using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.ClientAggregate.Events;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DaySpaPet.WebApi.Core.Services;

public class DeactivateClientService : IDeactivateClientService {
    private readonly IRepository<Client> _repository;
    private readonly IMediator _mediator;
    private readonly ILogger<DeactivateClientService> _logger;

    public DeactivateClientService(IRepository<Client> repository,
            IMediator mediator,
            ILogger<DeactivateClientService> logger) {
        _repository = repository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result> DeactivateClient(int clientId, OriginClock originClock) {
        _logger.LogInformation("Deactivating Client {clientId}", clientId);
        var client = await _repository.GetByIdAsync(clientId);
        if (client == null)
            return Result.NotFound();

        // TODO: Kick off a saga that will handle the deactivation spanning any business steps
        // ultimately culminating in the ClientDeactivatedEvent being published and the
        // client being marked as inactive.
        client.UpdateStatus(ClientAccountStatus.Deactive);
        await _repository.UpdateAsync(client);

        var domainEvent = new ClientDeactivationRequestedEvent(clientId, originClock);
        await _mediator.Publish(domainEvent);
        return Result.Success();
    }
}