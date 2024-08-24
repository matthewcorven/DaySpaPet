using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Core.Services;
using DaySpaPet.WebApi.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.WebApi.Core;
public static class CoreServiceExtensions {
  public static IServiceCollection AddCoreServices(this IServiceCollection services) {
    //services.AddScoped<IToDoItemSearchService, ToDoItemSearchService>();
    services.AddScoped<IDeactivateClientService, DeactivateClientService>();

    // Ensure DI dependencies of this layer are registered, and with the correct lifetime
    services.AssertImplementationIsRegisteredAs<Serilog.ILogger>(ServiceLifetime.Singleton);

    return services;
  }
}