using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.Update;
public class UpdateClientHandler : ICommandHandler<UpdateClientCommand, Result<ClientDTO>> {
  private readonly IRepository<Client> _repository;
  private readonly AppUserRequestContext _appUserRequestContext;

  public UpdateClientHandler(IRepository<Client> repository, AppUserRequestContext appUserRequestContext) {
    _repository = repository;
    _appUserRequestContext = appUserRequestContext;
  }

  public async Task<Result<ClientDTO>> Handle(UpdateClientCommand request, CancellationToken cancellationToken) {
    var existingClient = await _repository.GetByIdAsync(request.ClientId, cancellationToken);
    if (existingClient == null) {
      return Result.NotFound();
    }

    var originClock = _appUserRequestContext.ClockSnapshot;

    existingClient.UpdateName(request.FirstName, request.LastName, originClock);
    existingClient.UpdatePhone(request.PhoneCountryCode, request.PhoneNumber, request.PhoneExtension);
    existingClient.UpdateStatus(ClientAccountStatus.FromValue(request.Status));
    existingClient.UpdateEmailAddress(request.EmailAddress!, originClock);

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