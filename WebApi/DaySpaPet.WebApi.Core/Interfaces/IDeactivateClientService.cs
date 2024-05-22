using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Core.Interfaces;

public interface IDeactivateClientService
{
	// This service and method exist to provide a place in which to fire domain events
	// when deleting this aggregate root entity
	public Task<Result> DeactivateClient(int clientId, OriginClock originClock);
}
