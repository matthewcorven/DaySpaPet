using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace DaySpaPet.NativePlat.Client
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
#if DEBUG
            if (Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") is null)
            {
                Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", Debugger.IsAttached ? "Development" : "Production");
            }
#endif
            var app = new Program();
            app.Run(args);
        }
    }
}
