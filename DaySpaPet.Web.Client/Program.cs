using DaySpaPet.Web;
using DaySpaPet.Web.Client;
using DaySpaPet.Web.Client.Services;
using DaySpaPet.Web.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

internal static class Program
{
    public static string ApplicationName = "DaySpaPet Web Client";

    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Add device specific services used by Razor Class Library (DaySpaPet.Web)
        builder.Services.AddScoped<IFormFactor, FormFactor>();

        builder.RootComponents.Add<DaySpaPet.Web.Routes>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddFluentUIComponents(options =>
                {
                    options.UseTooltipServiceProvider = true;
                });
        builder.Services.AddDaySpaAppComponents();

        builder.Services.AddSingleton<IHostEnvironment>(new WasmHostEnvironment());

        await builder.Build().RunAsync().ConfigureAwait(true);
    }
}
