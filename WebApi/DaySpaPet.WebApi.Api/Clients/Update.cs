using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Clients.Update;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// Update an existing Client.
/// </summary>
/// <remarks>
/// Update an existing Client by providing a fully defined replacement set of values.
/// See: https://stackoverflow.com/questions/60761955/rest-update-best-practice-put-collection-id-without-id-in-body-vs-put-collecti
/// </remarks>
public class Update : Endpoint<UpdateClientRequest, UpdateClientResponse>
{
	private readonly IRepository<Client> _repository;
	private readonly IMediator _mediator;
	private readonly AppUserRequestContext _appUserRequestContext;

	public Update(
			IRepository<Client> repository,
			IMediator mediator,
			AppUserRequestContext appUserRequestContext)
	{
		_repository = repository;
		_mediator = mediator;
		_appUserRequestContext = appUserRequestContext;
	}

	public override void Configure()
	{
		Put(UpdateClientRequest.Route);
		AllowAnonymous();
		Summary(s =>
		{
			// XML Docs are used by default but are overridden by these properties:
			//s.Summary = "Create a new Client.";
			//s.Description = "Create a new Client. A valid name is required.";
			s.ExampleRequest = new UpdateClientRequest
			{
				// TODO: Generate from Bogus. Ideally, it would share underlying structure
				// of faker used by database seeder, yet using its own seed local to this
				// line of code, thus ensuring future generated values are deterministic
				// so that they only change when properties are added/removed/changed.
				ClientId = 1,
				Id = 1,
				FirstName = "Frank",
				LastName = "Zappa",
				PhoneCountryCode = "+1",
				PhoneNumber = "555-555-5555",
				PhoneExtension = "123",
				EmailAddress = "frank@thezap.com"
			};
		});
	}

	public override async Task HandleAsync(
			UpdateClientRequest request,
			CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new UpdateClientCommand(
				request.ClientId,
				request.FirstName!, request.LastName!,
				request.PhoneCountryCode!, request.PhoneNumber!, request.PhoneExtension!,
				request.Status!, request.EmailAddress!, _appUserRequestContext.ClockSnapshot));

		if (result.Status == ResultStatus.NotFound)
		{
			await SendNotFoundAsync(cancellationToken);
			return;
		}


		if (result.IsSuccess)
		{
			var dto = result.Value;
			Response = new UpdateClientResponse(new ClientRecord(
					dto.Id,
					dto.FirstName, dto.LastName,
					dto.PhoneCountryCode, dto.PhoneNumber, dto.PhoneExtension,
					dto.Status, dto.EmailAddress));
			return;
		}
	}
}
