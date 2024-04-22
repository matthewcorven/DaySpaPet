using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DaySpaPet.UseCases.Settings.Domain;

namespace DaySpaPet.NativePlat.Client;

public partial class MainPage : ContentPage
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MainPage> _logger;
    public MainPage(
        IHostEnvironment hostEnvironment,
        IConfiguration configuration,
        ILogger<MainPage> logger
        )
    {
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
        _logger = logger;

        var debugSettings = configuration.GetSection("DebugSettings").Get<DebugSettings>();
        if (debugSettings is not null)
        {
            _logger.LogInformation("Loaded {MainPage} in {EnvironmentName} with DebugSettings.KeyThree = {EnvironmentName} environment.", nameof(MainPage), _hostEnvironment.EnvironmentName, debugSettings!.KeyThree.Message);
        }
        else
        {
            _logger.LogInformation("Loaded {MainPage} in {EnvironmentName} with no DebugSettings.", nameof(MainPage), _hostEnvironment.EnvironmentName);
        }

        InitializeComponent();
    }
}