using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.Razor.Shared;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add common services required by the Fluent UI Web Components for Blazor library
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Library configuration</param>
    public static IServiceCollection AddDaySpaAppComponents(this IServiceCollection services, RootAppConfiguration? configuration = null)
    {
        //services.AddScoped<GlobalState>();
        //services.AddScoped<IToastService, ToastService>();
        //services.AddScoped<IDialogService, DialogService>();
        //services.AddScoped<IMessageService, MessageService>();

        //LibraryConfiguration options = configuration ?? new();
        //if (options.UseTooltipServiceProvider)
        //{
        //    services.AddScoped<ITooltipService, TooltipService>();
        //}
        //services.AddSingleton(options);

        //services.AddDesignTokens();


        return services;
    }

    /// <summary>
    /// Add common services required by the Fluent UI Web Components for Blazor library
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Library configuration</param>
    public static IServiceCollection AddDaySpaAppComponents(this IServiceCollection services, Action<RootAppConfiguration> configuration)
    {
        RootAppConfiguration options = new();
        configuration.Invoke(options);

        return AddDaySpaAppComponents(services, options);
    }
}
