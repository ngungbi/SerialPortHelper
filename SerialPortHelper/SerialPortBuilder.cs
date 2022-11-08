using System.Text;

namespace Ngb.SerialPortHelper;

public delegate void PortWriter(string data);

public delegate void TriggerDelegate(SerialPortWriter writer);

public delegate bool IdentifierDelegate(SerialPortReader reader, SerialPortWriter writer);

public sealed class SerialPortIdentifier {
    public string Name { get; set; } = string.Empty;

    // public string CommandString {
    //     get => Encoding.UTF8.GetString(CommandBytes);
    //     set => CommandBytes = Encoding.UTF8.GetBytes(value);
    // }
    //
    // public byte[] CommandBytes { get; set; } = Array.Empty<byte>();
    public TriggerDelegate? Trigger { get; set; }
    public IdentifierDelegate Identifier { get; set; } = null!;
    internal bool IsIdentified { get; set; }
}

public sealed class SerialPortConfiguration {
    // private IList<SerialPort> Ports = new List<SerialPort>();
    private readonly List<SerialPortIdentifier> _identifiers = new();
    internal TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(3);
    internal TimeSpan TriggerDelay { get; private set; } = TimeSpan.FromSeconds(2);
    internal int DefaultBaudRate { get; private set; } = 115200;
    internal int[] BaudRatesToTest { get; private set; } = Array.Empty<int>();
    internal Func<string, bool>? FilterCondition { get; private set; }

    public IReadOnlyList<SerialPortIdentifier> Identifiers => _identifiers;

    public SerialPortConfiguration Filter(Func<string, bool> filter) {
        FilterCondition = filter;
        return this;
    }

    public SerialPortConfiguration WithDefaultBaudRate(int baudRate) {
        DefaultBaudRate = baudRate;
        return this;
    }

    public SerialPortConfiguration TryBaudRates(params int[] baudRates) {
        BaudRatesToTest = baudRates;
        return this;
    }

    public SerialPortConfiguration SetTimeout(TimeSpan timeout) {
        Timeout = timeout;
        return this;
    }

    public SerialPortConfiguration SetTimeout(int seconds) {
        Timeout = TimeSpan.FromSeconds(seconds);
        return this;
    }

    public SerialPortConfiguration SetTriggerDelay(int milliseconds) {
        TriggerDelay = TimeSpan.FromMilliseconds(milliseconds);
        return this;
    }

    public SerialPortConfiguration AddPort(SerialPortIdentifier identifier) {
        if (identifier.Identifier == null) throw new NullReferenceException();
        _identifiers.Add(identifier);
        return this;
    }

    public SerialPortConfiguration AddPortManual(string name, string portName, int baudRate) {
        return this;
    }

    public SerialPortConfiguration AddPort(string name, IdentifierDelegate identifier) {
        if (identifier == null) throw new NullReferenceException();
        _identifiers.Add(new SerialPortIdentifier {Name = name, Identifier = identifier});
        return this;
    }

    public SerialPortConfiguration AddPort<T>(IdentifierDelegate action) {
        return AddPort(typeof(T).FullName!, action);
    }

    public SerialPortConfiguration AddPort<T>(TriggerDelegate trigger, IdentifierDelegate identifier) {
        if (identifier == null) throw new NullReferenceException();

        return AddPort(typeof(T).FullName!, identifier, trigger);
    }

    public SerialPortConfiguration AddPort(string name, IdentifierDelegate identifier, TriggerDelegate trigger) {
        if (identifier == null || trigger == null) throw new NullReferenceException();

        var portIdentifier = new SerialPortIdentifier {
            Name = name,
            Trigger = trigger,
            Identifier = identifier
        };
        _identifiers.Add(portIdentifier);
        return this;
    }
}
