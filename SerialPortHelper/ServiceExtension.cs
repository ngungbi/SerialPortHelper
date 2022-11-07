using Microsoft.Extensions.DependencyInjection;

namespace Ngb.SerialPortHelper;

public static class ServiceExtension {
    public static IServiceCollection AddSerialPort(this IServiceCollection services, Action<SerialPortConfiguration>? configure) {
        var configuration = new SerialPortConfiguration();
        configure?.Invoke(configuration);
        services.AddSingleton<ISerialPortProvider>(new SerialPortProvider(configuration));
        return services;
    }
}
