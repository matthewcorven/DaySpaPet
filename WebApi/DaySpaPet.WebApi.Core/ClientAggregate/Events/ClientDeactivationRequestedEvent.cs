using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a client is deleted.
/// The DeleteClientService is used to dispatch this event.
/// </summary>
internal class ClientDeactivationRequestedEvent : DomainEventBase
{
  public int ClientId { get; set; }

  public ClientDeactivationRequestedEvent(int clientId, OriginClock originClock) : base(originClock)
  {
    ClientId = clientId;
  }
}
