using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.SharedKernel;
using DaySpaPet.Core.ClientAggregate;

namespace DaySpaPet.UseCases.Clients.Create;

public sealed class CreateClientHandler : ICommandHandler<CreateClientCommand, Result<int>>
{
  private readonly IRepository<Client> _repository;

  public CreateClientHandler(IRepository<Client> repository)
  {
    _repository = repository;
  }

  public async Task<Result<int>> Handle(CreateClientCommand request,
    CancellationToken cancellationToken)
  {
    var newClient = new Client(request.firstName, request.lastName, 
      request.phoneCountryCode, request.phoneNumber, request.phoneExtension, 
      request.emailAddress, request.createdAt);
    var createdItem = await _repository.AddAsync(newClient, cancellationToken);

    return createdItem.Id;
  }
}
