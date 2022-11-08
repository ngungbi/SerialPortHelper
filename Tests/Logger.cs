using Microsoft.Extensions.Logging;
using Ngb.SerialPortHelper;

namespace Tests; 

public class Disposable : IDisposable {
    public void Dispose() { }
}

public class Logger : ILogger<SerialPortProvider> {
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    public bool IsEnabled(LogLevel logLevel) => true;
    public IDisposable BeginScope<TState>(TState state) => new Disposable();
}
