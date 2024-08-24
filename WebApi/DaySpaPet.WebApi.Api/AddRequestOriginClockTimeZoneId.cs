using Ardalis.GuardClauses;
using DaySpaPet.WebApi.Api.Endpoints.Behavior;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

internal sealed class AddRequestOriginClockTimeZoneId : IOperationProcessor {
  public sealed record RequestOriginClockTimeProcessorOptions {
    public bool Strict { get; set; } = false;
  }
  public AddRequestOriginClockTimeZoneId(Action<RequestOriginClockTimeProcessorOptions>? configure = null) {
    if (configure != null) {
      RequestOriginClockTimeProcessorOptions settings = new RequestOriginClockTimeProcessorOptions();
      configure?.Invoke(settings);
      if (settings.Strict) {
        Guard.Against.NullOrEmpty(RuntimeEnvironmentValues.HttpRequestHeader, Constants.HttpRequestHeaderKey);
      }
    }
  }

  public static class RuntimeEnvironmentValues {
    public static readonly string? HttpRequestHeader = Environment.GetEnvironmentVariable("HTTP_REQ_HEADER__ORIGIN_CLOCK_TZDBID");
  }

  public bool Process(OperationProcessorContext context) {
    OpenApiParameter RequestHeader_OriginClockTZDBID = new OpenApiParameter() {
      Name = Constants.HttpRequestHeaderKey,
      Kind = OpenApiParameterKind.Header,
      IsRequired = true,
      Type = JsonObjectType.String,
      Default = "America/New_York",
      Description = """
IANA/TZDB time zone id of the HTTP requestor, which may be human 
reporting their local area, or in the case of a service principal 
it represents the deployment name. 
Refer to https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
""".Trim()
    };

    context.OperationDescription.Operation.Parameters.Add(RequestHeader_OriginClockTZDBID);

    return true;
  }
}