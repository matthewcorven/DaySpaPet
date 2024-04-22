using Android.App;
using Android.Runtime;
using System.Diagnostics;

namespace DaySpaPet.NativePlat.Client
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
#if DEBUG
            if (Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") is null)
            {
                // If the developer didn't set their environment variable, we'll
                // set it for them based on whether they're debugging or not.
                Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", Debugger.IsAttached 
                    ? "Development" 
                    : "Production");
            }
#endif
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
