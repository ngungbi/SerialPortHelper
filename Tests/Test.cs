using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Ngb.SerialPortHelper;

namespace Tests;

public class GrblIdentificationTest {
    private ISerialPortProvider _serialPortProvider = null!;

    [SetUp]
    public void Setup() {
        var configure = new SerialPortConfiguration();
        configure.AddPort("1", (w, r) => Helper.Identify(ref w, ref r, "1000"));
        configure.AddPort("3", (w, r) => Helper.Identify(ref w, ref r, "3000"));

        configure.SetTimeout(TimeSpan.FromSeconds(10)).SetTriggerDelay(2000);
        _serialPortProvider = new SerialPortProvider(configure, new Logger());
    }

    [Test]
    public async Task Test() {
        var sw = Stopwatch.StartNew();
        await _serialPortProvider.WaitAsync();
        var elapsed = sw.Elapsed;
        Console.WriteLine(elapsed);
        var port = _serialPortProvider.GetPort("3");
        Assert.IsNotNull(port);
    }
}
