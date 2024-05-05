using DaySpaPet.WebApi.SharedKernel;
using NodaTime;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;
public class ClientNameUpdatedEvent : DomainEventBase
{
  public ClientNameUpdatedEvent(Client client, string newFirstName, string newLastName, string oldFirstName, string oldLastName, OriginClock originClock) : base(originClock)
  {
    Client = client;
    NewFirstName = newFirstName;
    NewLastName = newLastName;
    OldFirstName = oldFirstName;
    OldLastName = oldLastName;
  }
  public Client Client { get; private set; }
  public string NewFirstName { get; private set; }
  public string NewLastName { get; private set; }  
  public string OldFirstName { get; private set; }
  public string OldLastName { get; private set; }
}
