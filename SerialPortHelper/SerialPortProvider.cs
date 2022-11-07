using System.IO.Ports;
#if DEBUG
using System.Diagnostics;
#endif

namespace Ngb.SerialPortHelper;

public sealed class SerialPortMap : Dictionary<string, SerialPort> { }

public sealed class SerialPortProvider : ISerialPortProvider, IDisposable {
    private readonly List<SerialPort> _availablePorts = new();
    private readonly SerialPortConfiguration _configuration;
    private readonly SerialPortMap _portMap = new();
    private CancellationTokenSource _cts = new();
    private readonly TaskCompletionSource _tcs = new();
    private readonly Queue<int> _baudrates = new();

    public SerialPortProvider(SerialPortConfiguration configuration) {
        _configuration = configuration;
        foreach (int b in configuration.BaudRatesToTest) {
            _baudrates.Enqueue(b);
        }

        var portList = SerialPort.GetPortNames();
        foreach (string portName in portList) {
            var port = new SerialPort(portName, configuration.DefaultBaudRate);
            // try {
            port.DataReceived += DataReceived;
            port.DtrEnable = true;
            port.RtsEnable = true;

            // if (!port.IsOpen) {
            //     Thread.Sleep(100);
            //     port.Open();
            // }

            _availablePorts.Add(port);
            // } catch (IOException) {
            //     // Console.WriteLine(e);
            //     // throw;
            // }
        }

        foreach (var port in _availablePorts) {
            port.Open();
        }

        Task.Delay(_configuration.TriggerDelay).ContinueWith(_ => RunTrigger());

        WaitTimeout(); //.ContinueWith(_ => { });
    }

    private void TryBaudRate(int baudRate) {
#if DEBUG
        Console.WriteLine($"Try {baudRate}");
#endif
        // if(!_baudrates.TryDequeue(out var baud)) return;
        var unidentifiedPorts = new List<SerialPort>();
        foreach (var serialPort in _availablePorts) {
            if (_portMap.ContainsValue(serialPort)) continue;
            serialPort.BaudRate = baudRate;
            unidentifiedPorts.Add(serialPort);
        }

        _availablePorts.Clear();
        foreach (var serialPort in unidentifiedPorts) {
            _availablePorts.Add(serialPort);
            if (serialPort.IsOpen) serialPort.Close();
            serialPort.Open();
        }

        Task.Delay(_configuration.TriggerDelay).ContinueWith(_ => RunTrigger());
        _cts.Dispose();
        _cts = new CancellationTokenSource();
        WaitTimeout();
    }

    private void RunTrigger() {
        foreach (var identifier in _configuration.Identifiers) {
            if (identifier.IsIdentified || identifier.Trigger is null) {
                // Console.WriteLine("skip");
                continue;
            }

            foreach (var port in _availablePorts) {
#if DEBUG
                Console.WriteLine($"{identifier.Name} {port.PortName}");
#endif

                var writer = new SerialPortWriter(port);
                identifier.Trigger(writer);
            }
        }
    }

    private void DataReceived(object sender, SerialDataReceivedEventArgs e) {
        var port = (SerialPort) sender;
        // while (obj.BytesToRead > 0) {
        //     var line = obj.ReadLine();
        foreach (var identifier in _configuration.Identifiers) {
            if (identifier.IsIdentified) continue;

            var reader = new SerialPortReader(port);
            var writer = new SerialPortWriter(port);
            if (!identifier.Identifier(reader, writer)) continue;

            // obj.Close();
            _portMap.Add(identifier.Name, port);
            identifier.IsIdentified = true;
            port.DataReceived -= DataReceived;

            if (_configuration.Identifiers.Count == _portMap.Count) {
                _cts.Cancel();
            }

            return;
        }
        // }
    }

#if DEBUG
    private readonly Stopwatch _sw = Stopwatch.StartNew();
#endif

    private void WaitTimeout() {
        Delay(_configuration.Timeout, _cts.Token)
            .ContinueWith(_ => CloseUnusedPorts());
        // CloseUnusedPorts();
    }

    private static async Task Delay(TimeSpan delay, CancellationToken cancellationToken) {
        try {
            await Task.Delay(delay, cancellationToken);
        } catch (TaskCanceledException) { }
    }

    public Task WaitAsync() {
        return _tcs.Task;
    }

    private void CloseUnusedPorts() {
#if DEBUG
        Console.WriteLine("Closing ports...");
#endif
        foreach (var port in _availablePorts) {
            if (port.IsOpen) port.Close();
        }

        // _availablePorts.Clear();
        if (_baudrates.TryDequeue(out var baudRate)) {
            // CloseUnusedPorts();
            TryBaudRate(baudRate);
            return;
        }

        _tcs.SetResult();

#if DEBUG
        _sw.Stop();
        Console.WriteLine(_sw.Elapsed);
#endif
    }

    public SerialPort GetPort(string name) {
        return _portMap[name];
    }

    public SerialPort GetPort<T>() {
        var name = typeof(T).FullName!;
        if (_portMap.TryGetValue(name, out var port)) return port;
        throw new IndexOutOfRangeException("Port not available");
        // return _portMap[typeof(T).FullName!];
    }

    public void Dispose() {
        foreach (var item in _portMap) {
            try {
                var port = item.Value;
                if (port.IsOpen) port.Close();
                port.Dispose();
            } catch (Exception) {
                //
            }
        }

        _cts.Dispose();
    }
}
