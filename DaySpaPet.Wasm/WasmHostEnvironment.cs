using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DaySpaPet.Wasm;

internal sealed class WasmHostEnvironment : IHostEnvironment
{
    /// <summary>
    /// On other platforms we use the NETCORE_ENVIRONMENT environment variable,
    /// but on WebAssembly there are no environment variables in the browser!
    /// Perhaps one day there will be given that WASI will have access to the
    /// host's environment variables, and WebAssembly is a kindred technology to
    /// WASI.
    /// But for now, we'll hard-code to Production and override via compiler
    /// directives in Program.
    /// 
    /// This makes the property less about "environment" and more about "build configuration",
    /// and it's an opinionated choice that we're making here.
    /// </summary>
    public string EnvironmentName { get; set; } = 
#if DEBUG
      Environments.Development;
#else
      Environments.Production;
#endif

    public string ApplicationName { get; set; } = Program.ApplicationName;
    public string ContentRootPath { get; set; } = System.AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = null!;

    public bool IsDevelopment() => EnvironmentName == Environments.Development;
    public bool IsProduction() => EnvironmentName == Environments.Production;
    public bool IsStaging() => EnvironmentName == Environments.Staging;
}
