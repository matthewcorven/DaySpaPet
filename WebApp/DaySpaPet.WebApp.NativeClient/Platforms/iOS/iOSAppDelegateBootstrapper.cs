using Foundation;

namespace DaySpaPet.NativePlat.Client
{
    [Register("iOSAppDelegateBootstrapper")]
#pragma warning disable IDE1006 // Naming Styles
    public class iOSAppDelegateBootstrapper : MauiUIApplicationDelegate
#pragma warning restore IDE1006 // Naming Styles
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
