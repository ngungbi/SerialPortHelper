using System.IO.Ports;

namespace Ngb.SerialPortHelper; 

public interface ISerialPortProvider {
    public SerialPort GetPort(string name);

    SerialPort GetPort<T>();
    Task WaitAsync();
}
