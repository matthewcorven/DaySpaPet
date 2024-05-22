using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.UseCases.Clients.Create;

public sealed class CreateClientHandler : ICommandHandler<CreateClientCommand, Result<int>>
{
	private readonly IRepository<Client> _repository;

	public CreateClientHandler(
			IRepository<Client> repository)
	{
		_repository = repository;
	}

	public async Task<Result<int>> Handle(CreateClientCommand request,
			CancellationToken cancellationToken)
	{
		var newClient = new Client(
				request.FirstName,
				request.LastName,
				request.PhoneCountryCode,
				request.PhoneNumber,
				request.PhoneExtension,
				request.EmailAddress,
				request.OriginClock);
		var createdItem = await _repository.AddAsync(newClient, cancellationToken);

		return createdItem.Id;
	}
}
