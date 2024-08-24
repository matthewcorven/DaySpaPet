using Microsoft.Extensions.DependencyInjection;

namespace DaySpaPet.WebApi.SharedKernel;
public static class SharedKernelExtensions {
    public static IServiceCollection AddSharedKernel(this IServiceCollection services) {
        services.AddScoped<AppUserRequestContext>();

        // Ensure DI dependencies of this layer are registered, and with the correct lifetime
        services.AssertImplementationIsRegisteredAs<Serilog.ILogger>(ServiceLifetime.Singleton);

        return services;
    }
}