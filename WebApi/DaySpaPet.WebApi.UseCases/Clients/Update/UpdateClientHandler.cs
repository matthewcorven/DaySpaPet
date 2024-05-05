using Ardalis.Result;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.Core.ClientAggregate;

namespace DaySpaPet.WebApi.UseCases.Clients.Update;
public class UpdateClientHandler : ICommandHandler<UpdateClientCommand, Result<ClientDTO>>
{
  private readonly IRepository<Client> _repository;

  public UpdateClientHandler(IRepository<Client> repository)
  {
    _repository = repository;
  }

  public async Task<Result<ClientDTO>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
  {
    var existingClient = await _repository.GetByIdAsync(request.ClientId, cancellationToken);
    if (existingClient == null)
    {
      return Result.NotFound();
    }

    existingClient.UpdateName(request.FirstName, request.LastName);
    existingClient.UpdatePhone(request.PhoneCountryCode, request.PhoneNumber, request.PhoneExtension);
    existingClient.UpdateStatus(ClientAccountStatus.FromValue(request.Status));
    existingClient.UpdateEmailAddress(request.EmailAddress!);

    await _repository.UpdateAsync(existingClient, cancellationToken);

    return Result.Success(new ClientDTO(
      existingClient.Id, 
      existingClient.FirstName, 
      existingClient.LastName, 
      existingClient.PhoneCountryCode, 
      existingClient.PhoneNumber, 
      existingClient.PhoneExtension!, 
      existingClient.Status, 
      existingClient.EmailAddress!));
  }
}

