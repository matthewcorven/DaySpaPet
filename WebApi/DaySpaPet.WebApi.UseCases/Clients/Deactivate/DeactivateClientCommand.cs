using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.Deactivate;

public record DeactivateClientCommand(int ClientId, OriginClock originClock) : ICommand<Result>;
