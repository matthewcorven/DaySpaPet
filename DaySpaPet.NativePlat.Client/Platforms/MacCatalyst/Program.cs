using ObjCRuntime;
using System.Diagnostics;
using UIKit;

namespace DaySpaPet.NativePlat.Client
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
#if DEBUG
            if (Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") is null)
            {
                Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", Debugger.IsAttached 
                    ? "Development" 
                    : "Production");
            }
#endif
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(MacCatalystAppDelegateBootstrapper));
        }
    }
}