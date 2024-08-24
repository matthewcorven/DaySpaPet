using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.WebApi.SharedKernel;

public static class ServiceCollectionExtensions {
  public static void AssertImplementationIsRegisteredAs<T>(this IServiceCollection services, ServiceLifetime lifetime) {
    Type typed = typeof(T);
    ServiceDescriptor? svcDesc = services.FirstOrDefault(x => x.ServiceType == typed);
    {
      if (svcDesc == null) {
        throw new InvalidOperationException($"Unable to resolve service for type {typed} has not been registered in the service collection. Verify that AddHttpContextAccessor() is called before AddInfrastructureServices().");
      }
      if (svcDesc.Lifetime != lifetime) {
        throw new InvalidOperationException($"The service for type {typed} must be registered as {lifetime}. Verify that AddHttpContextAccessor() is called with the correct lifetime before AddInfrastructureServices().");
      }
    }
    return;
  }
}