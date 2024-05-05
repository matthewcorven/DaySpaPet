using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

public sealed class ClientEmailAddressUpdatedEvent : DomainEventBase
{
  public ClientEmailAddressUpdatedEvent(Client client, string newEmailAddress, string? oldEmailAddress, OriginClock originClock) : base(originClock)
  {
    Client = client;
    NewEmailAddress = newEmailAddress;
    OldEmailAddress = oldEmailAddress;
  }
  public Client Client { get; private set; }
  public string NewEmailAddress { get; private set; }
  public string? OldEmailAddress { get; private set; }
}
