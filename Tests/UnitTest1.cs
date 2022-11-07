using System.Net;
using Ngb.SerialPortHelper;

namespace Tests;

public class Tests {
    private ISerialPortProvider _serialPortProvider = null!;

    [SetUp]
    public void Setup() {
        var configure = new SerialPortConfiguration();
        // configure.TryBaudRates(57600, 38400);
        configure.AddPort<Tests>(EncoderIdentifier);
        configure.AddPort("Grbl", GrblIdentifier, GrblTrigger);
        configure.SetTimeout(TimeSpan.FromSeconds(10)).SetTriggerDelay(2000);
        _serialPortProvider = new SerialPortProvider(configure);
    }

    private static bool EncoderIdentifier(SerialPortReader reader, SerialPortWriter writer) {
        // return (x, w) => {
        // w.WriteLine("`");
        foreach (string line in reader.ReadAllLines()) {
            if (line.ToLower().Contains("enc")) return true;
        }

        // w.WriteLine("$$");
        return false;
        // };
    }

    private static void GrblTrigger(SerialPortWriter writer) {
        // Thread.Sleep(1000);
        // writer.Port.WriteLine("$X");
        Console.WriteLine("$$");
        writer.WriteLine("$");
    }

    private static bool GrblIdentifier(SerialPortReader reader, SerialPortWriter writer) {
        foreach (string line in reader.ReadAllLines()) {
            Console.Write(line);
            if (line.Contains("$$")) return true;
            if (line.Contains("$X")) writer.WriteLine("$X");
            // if (line.ToLower().Contains("grbl")) return true;
        }

        return false;
    }

    [Test]
    public void Test1() {
        // Thread.Sleep(5_000);
        _serialPortProvider.WaitAsync().Wait();
        var port = _serialPortProvider.GetPort<Tests>();
        // var grbl = _serialPortProvider.GetPort("Grbl");
        Assert.Pass();
    }

    [Test]
    public void Test() {
        Console.WriteLine(new Uri("192.168.1.12:1234"));
        var url = Uri.TryCreate("192.168.1.12:1234", UriKind.Absolute, out var result) ? result : null;
        Console.WriteLine(url);
        IPAddress.TryParse("192.168.1.12:1234", out var ip);
        Console.WriteLine(ip);
    }
}
