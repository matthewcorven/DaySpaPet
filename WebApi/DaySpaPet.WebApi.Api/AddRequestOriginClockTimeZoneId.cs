using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

internal class AddRequestOriginClockTimeZoneId : IOperationProcessor
{
  public bool Process(OperationProcessorContext context)
    {
        var hdrParameter = new OpenApiParameter()
        {
            Name = "X-Origin-TimeZoneId",
            Kind = OpenApiParameterKind.Header,
            IsRequired = true,
            Type = JsonObjectType.String,
            Default = "America/New_York",
            Description = "IANA/TZDB time zone id."
        };

        context.OperationDescription.Operation.Parameters.Add(hdrParameter);

        return true;
    }
}
