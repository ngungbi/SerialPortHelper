using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ngb.SerialPortHelper;

public static class ServiceExtension {
    public static IServiceCollection AddSerialPort(this IServiceCollection services, Action<SerialPortConfiguration>? configure) {
        var configuration = new SerialPortConfiguration();
        configure?.Invoke(configuration);
        services.TryAddSingleton(configuration);
        services.TryAddSingleton<ISerialPortProvider, SerialPortProvider>();
        return services;
    }
}
