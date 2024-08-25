using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.ClientAggregate.Specifications;
using DaySpaPet.WebApi.SharedKernel;
using EntityFramework.Exceptions.Common;

namespace DaySpaPet.WebApi.UseCases.Clients.Create;

public sealed class CreateClientHandler : ICommandHandler<CreateClientCommand, Result<int>> {
  private readonly IRepository<Client> _repository;

  public CreateClientHandler(
          IRepository<Client> repository) {
    _repository = repository;
  }

  public async Task<Result<int>> Handle(CreateClientCommand request,
          CancellationToken cancellationToken) {
    if (await _repository.SingleOrDefaultAsync(new ClientByEmailAddressSpec(request.EmailAddress), cancellationToken) != null) {
      return Result<int>.Conflict("Client with this email address already exists");
    }

    Client newClient = new(
            request.FirstName,
            request.LastName,
            request.PhoneCountryCode,
            request.PhoneNumber,
            request.PhoneExtension,
            request.EmailAddress,
            request.OriginClock);

    try {
      Client createdItem = await _repository.AddAsync(newClient, cancellationToken);
      return createdItem.Id;
    } catch (UniqueConstraintException) {
      return Result<int>.Conflict("Client with this email address already exists");
    }
  }
}