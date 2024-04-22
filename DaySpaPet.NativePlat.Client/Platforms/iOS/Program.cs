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
            // If the developer didn't set their environment variable, we'll
            // set it for them based on whether they're debugging or not.
            Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", Debugger.IsAttached 
                ? "Development" 
                : "Production");
#endif
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(iOSAppDelegateBootstrapper));
        }
    }
}
