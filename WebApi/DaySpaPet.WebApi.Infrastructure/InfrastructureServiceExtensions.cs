using DaySpaPet.WebApi.Core.Interfaces;
using DaySpaPet.WebApi.Infrastructure.CoreImplementations;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.Infrastructure.Data.Queries;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace DaySpaPet.WebApi.Infrastructure;



public static class InfrastructureServiceExtensions {
    public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            bool isDevelopment,
            string connectionString) {
        // Ensure DI dependencies of this layer are registered, and with the correct lifetime
        services.AssertImplementationIsRegisteredAs<Serilog.ILogger>(ServiceLifetime.Singleton);
        services.AssertImplementationIsRegisteredAs<IHttpContextAccessor>(ServiceLifetime.Singleton);

        services.AddSingleton<IClock, DaySpaPetClock>();
        services.AddSingleton<IGlobalizationService, DaySpaPetGlobalizationService>();

        if (isDevelopment) {
            RegisterDevelopmentOnlyDependencies(services);
        } else {
            RegisterProductionOnlyDependencies(services);
        }

        RegisterEF(services, connectionString);


        return services;
    }

    private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services) {
        //services.AddScoped<IEmailSender, SmtpEmailSender>();
        //services.AddScoped<IListClientsQueryService, FakeListClientsQueryService>();
        //services.AddScoped<IListIncompleteItemsQueryService, FakeListIncompleteItemsQueryService>();
        services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
    }

    private static void RegisterProductionOnlyDependencies(IServiceCollection services) {
        //services.AddScoped<IEmailSender, SmtpEmailSender>();
        //services.AddScoped<IListClientsQueryService, ListClientsQueryService>();
        //services.AddScoped<IListIncompleteItemsQueryService, ListIncompleteItemsQueryService>();
        services.AddScoped<IListClientsShallowQueryService, ListClientsShallowQueryService>();
    }

    private static void RegisterEF(IServiceCollection services, string connectionString) {
        services.AddDbContext<AppDbContext>(
        (sp, options) => options
                        .UseSqlServer(connectionString, o => o.UseNodaTime()));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    }
}