using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaySpaPet.WebApi.Core;
public static class CoreServiceExtensions
{
  public static IServiceCollection AddCoreServices(this IServiceCollection services, ILogger logger)
  {
    //services.AddScoped<IToDoItemSearchService, ToDoItemSearchService>();
    services.AddScoped<IDeactivateClientService, DeactivateClientService>();
    
    logger.LogInformation("{Project} services registered", nameof(DaySpaPet.WebApi.Core));

    return services;
  }
}
