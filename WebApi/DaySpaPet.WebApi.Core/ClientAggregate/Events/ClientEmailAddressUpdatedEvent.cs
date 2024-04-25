using Ardalis.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

public sealed class ClientEmailAddressUpdatedEvent(Client client, string newEmailAddress, string? oldEmailAddress) : DomainEventBase
{
  public Client Client { get; } = client;
  public string NewEmailAddress { get; } = newEmailAddress;
  public string? OldEmailAddress { get; } = oldEmailAddress;
}
