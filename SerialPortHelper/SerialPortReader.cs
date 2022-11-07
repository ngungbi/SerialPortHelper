using System.IO.Ports;

namespace Ngb.SerialPortHelper;

public readonly struct SerialPortReader {
    private readonly SerialPort _port;
    public SerialPortReader(SerialPort port) { _port = port; }

    public bool DataAvailable => _port.BytesToRead > 0;

    public string ReadLine() {
        return _port.ReadLine();
    }


    public IEnumerable<string> ReadAllLines() {
        while (_port.BytesToRead > 0) {
            yield return _port.ReadLine();
        }
    }
}
