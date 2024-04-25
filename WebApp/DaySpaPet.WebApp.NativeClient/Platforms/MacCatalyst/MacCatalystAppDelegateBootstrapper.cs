using Foundation;

namespace DaySpaPet.NativePlat.Client
{
    [Register("MacCatalystAppDelegateBootstrapper")]
    public class MacCatalystAppDelegateBootstrapper : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
