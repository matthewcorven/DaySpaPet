using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Clients.Create;
using FastEndpoints;
using MediatR;

namespace DaySpaPet.WebApi.Api.Endpoints.Clients;

/// <summary>
/// Create a new Client
/// </summary>
/// <remarks>
/// Creates a new Client given a name.
/// </remarks>
public class Create : Endpoint<CreateClientRequest, CreateClientResponse> {
  private readonly IMediator _mediator;
  private readonly AppUserRequestContext _appUserRequestContext;

  public Create(IMediator mediator, AppUserRequestContext appUserRequestContext) {
    _mediator = mediator;
    _appUserRequestContext = appUserRequestContext;
  }

  public override void Configure() {
    Post(CreateClientRequest.Route);
    AccessControl( //Permission for creating new articles in the system.
            keyName: "Client_Create",
            behavior: Apply.ToThisEndpoint,
            groupNames: ["Groomer", "Owner", "Admin"]);
    Summary(s => {
      // XML Docs are used by default but are overridden by these properties:
      //s.Summary = "Create a new Client.";
      //s.Description = "Create a new Client. A valid name is required.";
      s.ExampleRequest = new CreateClientRequest {
        // TODO: Generate from Bogus. Ideally, it would share underlying structure
        // of faker used by database seeder, yet using its own seed local to this
        // line of code, thus ensuring future generated values are deterministic
        // so that they only change when properties are added/removed/changed.
        FirstName = "Frank",
        LastName = "Zappa",
        PhoneCountryCode = "+1",
        PhoneNumber = "555-555-5555",
        PhoneExtension = "123",
        EmailAddress = "frank@thezap.com"
      };
    });
  }

  public override async Task HandleAsync(CreateClientRequest req, CancellationToken ct) {
    Ardalis.Result.Result<int> result = await _mediator.Send(new CreateClientCommand(
                    req.FirstName!,
                    req.LastName!,
                    req.PhoneCountryCode!,
                    req.PhoneNumber!,
                    req.PhoneExtension!,
                    req.EmailAddress!,
                    _appUserRequestContext.ClockSnapshot)
            , ct);

    if (result.IsSuccess) {
      Response = new CreateClientResponse(result.Value);
      return;
    }
  }
}