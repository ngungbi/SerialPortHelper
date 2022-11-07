using System.IO.Ports;

namespace Ngb.SerialPortHelper;

public readonly ref struct SerialPortWriter {
    private readonly SerialPort _port;
    public SerialPortWriter(SerialPort port) { _port = port; }

    public SerialPort Port => _port;

    public void WriteLine(string text) {
        // if (!_port.IsOpen) return;
        _port.WriteLine(text);
    }

    public void WriteChars(char[] chars) {
        if (chars.Length == 0) return;
        _port.Write(chars, 0, chars.Length);
    }

    public void WriteChars(ReadOnlySpan<char> chars) {
        if (chars.Length == 0) return;
        _port.Write(chars.ToArray(), 0, chars.Length);
    }

    public void WriteBytes(byte[] bytes) {
        if (bytes.Length == 0) return;
        _port.Write(bytes, 0, bytes.Length);
    }

    public void WriteBytes(ReadOnlySpan<byte> bytes) {
        if (bytes.Length == 0) return;
        _port.Write(bytes.ToArray(), 0, bytes.Length);
    }
}
