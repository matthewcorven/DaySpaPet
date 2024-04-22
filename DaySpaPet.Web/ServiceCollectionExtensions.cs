using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.Web;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add common services required by this RazorShared library
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Library configuration</param>
    public static IServiceCollection AddDaySpaAppComponents(this IServiceCollection services, RazorSharedConfiguration? configuration = null)
    {
        RazorSharedConfiguration options = configuration ?? new();
        if (options.SomeBehaviorOrStrategy)
        {
            // services.AddScoped<IAnotherService, AnotherService>();
        }
        services.AddSingleton(options);

        return services;
    }

    /// <summary>
    /// Add common services required by this RazorShared library
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Library configuration</param>
    public static IServiceCollection AddDaySpaAppComponents(this IServiceCollection services, Action<RazorSharedConfiguration> configuration)
    {
        RazorSharedConfiguration options = new();
        configuration.Invoke(options);

        return AddDaySpaAppComponents(services, options);
    }
}
