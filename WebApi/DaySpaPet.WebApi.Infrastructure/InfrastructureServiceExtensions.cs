using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using SimplerSoftware.EntityFrameworkCore.SqlServer.NodaTime;
using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Infrastructure.CoreImplementations;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.Infrastructure.Data.Queries;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NodaTime;
using EntityFramework.Exceptions.SqlServer;

namespace DaySpaPet.WebApi.Infrastructure;

public static class InfrastructureServiceExtensions {

  public static IServiceCollection AddInfrastructureServices(
        this WebApplicationBuilder builder) {
    IServiceCollection services = builder.Services;

    // Ensure DI dependencies of this layer are registered, and with the correct lifetime
    services.AssertImplementationIsRegisteredAs<Serilog.ILogger>(ServiceLifetime.Singleton);
    services.AssertImplementationIsRegisteredAs<IHttpContextAccessor>(ServiceLifetime.Singleton);

    // Register things which this layer provides an implementation for
    services.AddSingleton<IClock, DaySpaPetClock>();
    services.AddSingleton<IGlobalizationService, DaySpaPetGlobalizationService>();
    services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
    services.AddScoped<IAppUserAuthenticationService, DaySpaUserAuthenticationService>();

    // During development we want to have fake implementations
    bool isDevelopment = builder.Environment.IsDevelopment();
    if (isDevelopment) {
      RegisterDevelopmentOnlyDependencies(services);
    } else {
      RegisterProductionOnlyDependencies(services);
    }

    builder.AddSqlServerDbContext<AppDbContext>("DaySpaPetDb", configureDbContextOptions: o => {
      o.UseExceptionProcessor();
      o.UseNodaTime();
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    return services;
  }

  private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services) {
    //services.AddScoped<IEmailSender, FakeSmtpEmailSender>();
    //services.AddScoped<IListClientsQueryService, FakeListClientsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, FakeListIncompleteItemsQueryService>();
  }

  private static void RegisterProductionOnlyDependencies(IServiceCollection services) {
    //services.AddScoped<IEmailSender, SmtpEmailSender>();
    //services.AddScoped<IListClientsQueryService, ListClientsQueryService>();
    //services.AddScoped<IListIncompleteItemsQueryService, ListIncompleteItemsQueryService>();
  }
}