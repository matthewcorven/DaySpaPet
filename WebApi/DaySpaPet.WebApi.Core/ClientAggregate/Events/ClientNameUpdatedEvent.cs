using Ardalis.SharedKernel;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;
public class ClientNameUpdatedEvent(Client client, string newFirstName, string newLastName, string oldFirstName, string oldLastName) : DomainEventBase
{
  public Client Client { get; } = client;
  public string NewFirstName { get; } = newFirstName;
  public string NewLastName { get; } = newLastName;
  public string OldFirstName { get; } = oldFirstName;
  public string OldLastName { get; } = oldLastName;
}
