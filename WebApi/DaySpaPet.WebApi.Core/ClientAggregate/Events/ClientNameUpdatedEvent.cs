using DaySpaPet.WebApi.SharedKernel;
using NodaTime;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Events;
public class ClientNameUpdatedEvent(
		Client client,
		string newFirstName, string newLastName,
		string oldFirstName, string oldLastName,
		OriginClock originClock)
		: DomainEventBase(originClock)
{
	public Client Client { get; private set; } = client;
	public string NewFirstName { get; private set; } = newFirstName;
	public string NewLastName { get; private set; } = newLastName;
	public string OldFirstName { get; private set; } = oldFirstName;
	public string OldLastName { get; private set; } = oldLastName;
}
