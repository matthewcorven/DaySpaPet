using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DaySpaPet.NativePlat.Client;

internal sealed class MauiHostEnvironment : IHostEnvironment
{
    /// <summary>
    /// All of MAUI's supported platforms have access to the host's environment and they 
    /// all have environment variables, but we'll hard-code to Production and override via
    /// MauiProgram. 
    /// 
    /// This makes the property less about "environment" and more about "build configuration",
    /// and it's an opinionated choice that we're making here.
    /// </summary>
    public string EnvironmentName { get; set; } = Environments.Production;
    public string ApplicationName { get; set; } = DaySpaPet.NativePlat.Client.MauiProgram.ApplicationName;
    public string ContentRootPath { get; set; } = System.AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = null!;

    public bool IsDevelopment() => EnvironmentName == Environments.Development;
    public bool IsProduction() => EnvironmentName == Environments.Production;
    public bool IsStaging() => EnvironmentName == Environments.Staging;
}
