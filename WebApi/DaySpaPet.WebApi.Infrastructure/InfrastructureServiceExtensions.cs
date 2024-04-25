using Ardalis.SharedKernel;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.Infrastructure.Data.Queries;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaySpaPet.WebApi.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ILogger logger,
    bool isDevelopment)
  {
    if (isDevelopment)
    {
      RegisterDevelopmentOnlyDependencies(services);
    }
    else
    {
      RegisterProductionOnlyDependencies(services);
    }
    
    RegisterEF(services);
    
    logger.LogInformation("{Project} services registered", "Infrastructure");
    
    return services;
  }

  private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
  {
    //services.AddScoped<IEmailSender, SmtpEmailSender>();
    //services.AddScoped<IListContributorsQueryService, FakeListContributorsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, FakeListIncompleteItemsQueryService>();
    services.AddScoped<IListClientsShallowQueryService, FakeListClientsShallowQueryService>();
  }
  
  private static void RegisterProductionOnlyDependencies(IServiceCollection services)
  {
    //services.AddScoped<IEmailSender, SmtpEmailSender>();
    //services.AddScoped<IListContributorsQueryService, ListContributorsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, ListIncompleteItemsQueryService>();
    services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
  }

  private static void RegisterEF(IServiceCollection services)
  {
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
  }
}
