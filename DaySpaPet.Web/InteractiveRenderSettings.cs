using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace DaySpaPet.Web;

/// <summary>
/// Set the interactive render modes for use by components in the Shared class library 
/// which can be overridden by the client's hosting a Hybrid Blazor application, where
/// all things are already Interactive, therefore InteractiveServer, and InteractiveAuto
/// just don't make any sense.
/// This is a workaround for .NET 8 provided by Beth Massi and Eilon Lipton, see:
/// https://www.youtube.com/watch?v=hrXAkNsjaoI at 18:08.
/// https://github.com/BethMassi/HybridSharedUI
/// </summary>
public static class InteractiveRenderSettings
{
    public static IComponentRenderMode? InteractiveServer { get; set; } = RenderMode.InteractiveServer;
    public static IComponentRenderMode? InteractiveAuto { get; set; } = RenderMode.InteractiveAuto;
    public static IComponentRenderMode? InteractiveWebAssembly { get; set; } = RenderMode.InteractiveWebAssembly;

    public static void ConfigureBlazorHybridRenderModes()
    {
        InteractiveServer = null;
        InteractiveAuto = null;
        InteractiveWebAssembly = null;
    }
}