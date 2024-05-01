using Ardalis.Result;
using Ardalis.SharedKernel;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DaySpaPet.WebApi.UseCases.Clients.Create;

public sealed class CreateClientHandler : ICommandHandler<CreateClientCommand, Result<int>>
{
  private readonly IRepository<Client> _repository;
  private readonly IRequestOriginContextProvider _requestOriginContextProvider;

  public CreateClientHandler(
    IRepository<Client> repository,
    IRequestOriginContextProvider requestOriginContextProvider)
  {
    _repository = repository;
    _requestOriginContextProvider = requestOriginContextProvider;
  }

  public async Task<Result<int>> Handle(CreateClientCommand request,
    CancellationToken cancellationToken)
  {
    var originContext = _requestOriginContextProvider.GetOriginClock();
    var newClient = new Client(
      request.firstName, 
      request.lastName, 
      request.phoneCountryCode, 
      request.phoneNumber, 
      request.phoneExtension, 
      request.emailAddress, 
      originContext.IsDaylightSavingsTime, 
      originContext.TimeZoneId,
      originContext.LocalDateTime);
    var createdItem = await _repository.AddAsync(newClient, cancellationToken);

    return createdItem.Id;
  }
}
