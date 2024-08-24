using Ardalis.GuardClauses;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.SharedKernel;
using MediatR;
using NodaTime;

namespace DaySpaPet.WebApi.Api.Endpoints.Behavior;

public class CaptureUserRequestContextBehavior<TRequest, TResponse>
                : IPipelineBehavior<TRequest, TResponse>
                        where TRequest : IRequest<TResponse> {

  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IClock _clock;
  private readonly IGlobalizationService _globalizationService;
  private readonly AppUserRequestContext _appUserRequestContext;

  public CaptureUserRequestContextBehavior(IHttpContextAccessor httpContextAccessor,
          IClock clock,
          IGlobalizationService globalizationService,
                  AppUserRequestContext appUserRequestContext) {
    _httpContextAccessor = httpContextAccessor;
    _clock = clock;
    _globalizationService = globalizationService;
    _appUserRequestContext = appUserRequestContext;
  }

  public Task<TResponse> Handle(TRequest request,
                  RequestHandlerDelegate<TResponse> next,
                  CancellationToken cancellationToken
          ) {
    string? username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
    // Guard.Against.NullOrEmpty(username, "User UPN is required.");

    // Access request header
    IHeaderDictionary requestHeaders = _httpContextAccessor.HttpContext!.Request.Headers;
    string findHeader = Constants.HttpRequestHeaderKey;
    // Get the time zone ID from the request header
    requestHeaders.TryGetValue(findHeader, out Microsoft.Extensions.Primitives.StringValues timeZoneIdStringValues);
    Guard.Against.Null(timeZoneIdStringValues, $"HTTP request header \"{findHeader}\" is required.");
    string? timeZoneIdValue = timeZoneIdStringValues.FirstOrDefault();
    Guard.Against.NullOrWhiteSpace(timeZoneIdValue, $"HTTP request header \"{findHeader}\" value \"{timeZoneIdValue}\" is not understood to understand a string.");

    // Get the current UTC time
    Instant now = _clock.GetCurrentInstant();
    // Get the time zone from the ID
    if (!_globalizationService.TryGetTimeZoneById(timeZoneIdValue, out DateTimeZone? zone)) {
      throw new InvalidTimeZoneException($"Time zone ID \"{timeZoneIdValue}\" is not a valid time zone ID.");
    }

    // Get the ZonedDateTime in the specified zone
    ZonedDateTime zonedDateTime = now.InZone(zone!);

    // Convert to LocalDateTime
    LocalDateTime originLocalDateTime = zonedDateTime.LocalDateTime;
    bool isDst = zonedDateTime.IsDaylightSavingTime();

    OriginClock originClock = new OriginClock(originLocalDateTime, timeZoneIdValue, isDst);

    _appUserRequestContext.Set(username ?? "", [], originClock);

    return next();
  }

}