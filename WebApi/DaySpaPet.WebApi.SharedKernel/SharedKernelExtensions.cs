using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaySpaPet.WebApi.SharedKernel;
public static class SharedKernelExtensions
{
  public static IServiceCollection AddSharedKernel(this IServiceCollection services, ILogger logger)
  {
    services.AddScoped<AppUserRequestContext>();

    logger.LogInformation("{Project} services registered", nameof(DaySpaPet.WebApi.SharedKernel));

    return services;
  }
}
