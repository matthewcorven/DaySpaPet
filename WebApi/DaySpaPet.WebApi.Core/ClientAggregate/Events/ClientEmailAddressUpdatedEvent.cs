using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

public sealed class ClientEmailAddressUpdatedEvent(
        Client client, string newEmailAddress, string? oldEmailAddress, OriginClock originClock)
        : DomainEventBase(originClock) {
  public Client Client { get; private set; } = client;
  public string NewEmailAddress { get; private set; } = newEmailAddress;
  public string? OldEmailAddress { get; private set; } = oldEmailAddress;
}