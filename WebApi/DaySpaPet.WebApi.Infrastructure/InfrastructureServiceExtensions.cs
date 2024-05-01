using Ardalis.SharedKernel;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Infrastructure.CoreImplementations;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.Infrastructure.Data.Queries;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace DaySpaPet.WebApi.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ILogger logger,
    bool isDevelopment,
    string connectionString)
  {
    // Guard, at app startup
    ServiceDescriptor? httpContextAccessorServiceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IHttpContextAccessor));
    {
      // ... against inaccessible IHttpContextAccessor
      if (httpContextAccessorServiceDescriptor == null)
      {
        throw new InvalidOperationException($"Unable to resolve service for type {typeof(IHttpContextAccessor)} has not been registered in the service collection. Verify that AddHttpContextAccessor() is called before AddInfrastructureServices().");
      }
      // ... against non-Scoped IHttpContextAccessor
      if (httpContextAccessorServiceDescriptor.Lifetime != ServiceLifetime.Scoped)
      {
        throw new InvalidOperationException($"The service for type {typeof(IHttpContextAccessor)} must be registered as Scoped. Verify that AddHttpContextAccessor() is called with the correct lifetime before AddInfrastructureServices().");
      }
    }

    services.AddSingleton<IClock, DaySpaPetClock>();
    services.AddScoped<IRequestOriginContextProvider, RequestOriginContextProvider>();

    if (isDevelopment)
    {
      RegisterDevelopmentOnlyDependencies(services);
    }
    else
    {
      RegisterProductionOnlyDependencies(services);
    }

    RegisterEF(services, connectionString);

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }

  private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
  {
    //services.AddScoped<IEmailSender, SmtpEmailSender>();
    //services.AddScoped<IListContributorsQueryService, FakeListContributorsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, FakeListIncompleteItemsQueryService>();
    services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
  }

  private static void RegisterProductionOnlyDependencies(IServiceCollection services)
  {
    //services.AddScoped<IEmailSender, SmtpEmailSender>();
    //services.AddScoped<IListContributorsQueryService, ListContributorsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, ListIncompleteItemsQueryService>();
    services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
  }

  private static void RegisterEF(IServiceCollection services, string connectionString)
  {
    services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(connectionString, o => o.UseNodaTime()));

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
  }
}
