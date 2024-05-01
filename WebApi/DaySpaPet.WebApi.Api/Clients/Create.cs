using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.UseCases.Clients.Create;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Clients;

/// <summary>
/// Create a new Client
/// </summary>
/// <remarks>
/// Creates a new Client given a name.
/// </remarks>
public class Create : Endpoint<CreateClientRequest, CreateClientResponse>
{
  private readonly IMediator _mediator;

  public Create(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post(CreateClientRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      // XML Docs are used by default but are overridden by these properties:
      //s.Summary = "Create a new Client.";
      //s.Description = "Create a new Client. A valid name is required.";
      s.ExampleRequest = new CreateClientRequest
      {
        // TODO: Generate from Bogus. Ideally, it would be same instance of a
        // faker used by database seeder, yet using its own seed local to this
        // line of code, thus ensuring future generated values are deterministic
        // so that they only change when properties are added/removed/changed.
        FirstName = "Client Name"
      };
    });
  }

  public override async Task HandleAsync(
    CreateClientRequest request,
    CancellationToken cancellationToken)
  {
    IRequestOriginContextProvider rocP = Resolve<IRequestOriginContextProvider>();
    var result = await _mediator.Send(new CreateClientCommand(
        request.FirstName!,
        request.LastName!,
        request.PhoneCountryCode!,
        request.PhoneNumber!,
        request.PhoneExtension!,
        request.EmailAddress!,
        rocP.GetOriginClock())
      , cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateClientResponse(result.Value);
      return;
    }
  }
}
