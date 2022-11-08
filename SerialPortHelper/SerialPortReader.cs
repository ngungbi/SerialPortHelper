using System.IO.Ports;

namespace Ngb.SerialPortHelper;

public struct SerialPortReader {
    private readonly SerialPort _port;
    private readonly List<string> _buffer;
    private int _index;

    public SerialPortReader(SerialPort port, List<string> buffer) {
        _port = port;
        _buffer = buffer;
        _index = 0;
    }

    // public bool DataAvailable => _port.BytesToRead > 0;
    public string PortName => _port.PortName;

    public string ReadLine() {
        if (_index < _buffer.Count) {
            var line = _buffer[_index];
            _index++;
            return line;
        } else {
            return string.Empty;
            // var line = _port.ReadLine();
            // _buffer.Add(line);
            // return line;
        }
    }


    public IEnumerable<string> ReadAllLines() => _buffer;
    // {
    //     // if (_buffer.Count > 0) {
    //     while (_index < _buffer.Count) {
    //         yield return _buffer[_index];
    //         _index++;
    //     }
    //     // }
    //
    //     while (_port.BytesToRead > 0) {
    //         var line = _port.ReadLine();
    //         _buffer.Add(line);
    //         yield return line;
    //     }
    // }
}
