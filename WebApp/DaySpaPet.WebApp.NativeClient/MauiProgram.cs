using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Maui;
using DaySpaPet.Web;
using DaySpaPet.Web.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using DaySpaPet.NativePlat.Client.Services;

namespace DaySpaPet.NativePlat.Client;

internal static class MauiProgram
{
    public static string ApplicationName = "DaySpaPet Native Client";

    public static IServiceProvider? Services { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        // Ignore the interactive render settings in the Presentation Kernel class library,
        // as the client will be hosting a Hybrid Blazor application and here in MAUI,
        // everything is assumed to be running fully-interactively.
        InteractiveRenderSettings.ConfigureBlazorHybridRenderModes();

        var builder = MauiApp.CreateBuilder();

        // Add device specific services used by Razor Class Library (DaySpaPet.Web)
        builder.Services.AddScoped<IFormFactor, FormFactor>();

        builder.UseMauiApp<DaySpaPet.NativePlat.Client.App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        var assembly = Assembly.GetExecutingAssembly();
        var configBuilder = new ConfigurationBuilder();
        var resourceNames = assembly.GetManifestResourceNames();

        foreach (var resourceName in resourceNames)
        {
            Console.WriteLine(resourceName);
        }

        using var appSettingsStream = assembly.GetManifestResourceStream("DaySpaPet.NativePlat.Client.appsettings.json");
        configBuilder.AddJsonStream(appSettingsStream!);
#if DEBUG
        using var appSettingsDevStream = assembly.GetManifestResourceStream("DaySpaPet.NativePlat.Client.appsettings.Development.json");
        if (appSettingsDevStream is not null) configBuilder.AddJsonStream(appSettingsDevStream!);
#endif
        var config = configBuilder.Build();
        builder.Configuration.AddConfiguration(config);

        builder.Services.AddFluentUIComponents(options =>
        {
            options.UseTooltipServiceProvider = true;
        });
        builder.Services.AddDaySpaAppComponents();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var hostEnvironment = new MauiHostEnvironment();
#if DEBUG
        hostEnvironment.EnvironmentName = Environments.Development;
#endif
        builder.Services.AddSingleton<IHostEnvironment>(hostEnvironment);

        // Add any pages to the service collection to allow injection as dependency
        // such as in App.xaml.cs
        builder.Services.AddTransient<MainPage>();

        var app = builder.Build();
        Services = app.Services;
        return app;
    }
}
