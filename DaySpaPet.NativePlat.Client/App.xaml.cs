namespace DaySpaPet.NativePlat.Client;

#pragma warning disable CA1724
// Else "The type name App conflicts in whole or in part with the namespace name 'Android.App'."
public partial class App : Application
#pragma warning restore CA1724
{
    public App(MainPage mainPage)
    {
        InitializeComponent();

        MainPage = mainPage;
    }
}