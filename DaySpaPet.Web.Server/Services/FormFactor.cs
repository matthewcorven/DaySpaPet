using DaySpaPet.Web.Interfaces;

namespace DaySpaPet.Web.Server.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor()
    {
        return "Hosted Kestrel Server";
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}