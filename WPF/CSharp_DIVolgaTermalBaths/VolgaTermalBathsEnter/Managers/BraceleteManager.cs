using System.IO;
using System.IO.Ports;
using System.Text;
using Serilog;
using VolgaTermalBathsEnter.Models.Bracelete.Configuration;

namespace VolgaTermalBathsEnter.Managers;

public delegate void DataReceivedEventHandler(object sender, string data);

public class BraceletManager : IDisposable
{
    private SerialPort _serialPort = new();
    private bool _disposed;
    private PortModel _portModel;

    private string? path = "Logs/braceletLogging.txt";

    public event DataReceivedEventHandler DataReceived;

    public BraceletManager(PortModel portModel, ILogger logger)
    {
        _portModel = portModel;

        if (!File.Exists(path))
            File.Create(path).Dispose();

        try
        {
            InitializeSerialPort(_portModel);
            File.AppendAllText(path, $"{DateTime.Now} - Serial port initialized" + "\n");
            StartAutomaticScanning();
        }
        catch (Exception e)
        {
            File.AppendAllText(path,$"{DateTime.Now} - [ERROR]" + e.Message + "\n");
            logger.Error(e.Message);
        }
    }

    private void InitializeSerialPort(PortModel portModel)
    {
        _serialPort = new SerialPort
        {
            PortName = portModel.PortName,
            BaudRate = portModel.BaudRate,
            Parity = Parity.None,
            DataBits = portModel.DataBits,
            StopBits = StopBits.One,
            Encoding = Encoding.UTF8
        };

        _serialPort.DataReceived += DataReceivedHandler;
    }

    private void StartAutomaticScanning()
    {
        if (_serialPort.IsOpen) return;
        _serialPort.Open();
        File.AppendAllText(path, $"{DateTime.Now} - Serial port opened automatically for scanning." + "\n");
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        var sp = (SerialPort)sender;
        var inData = sp.ReadLine();
        File.AppendAllText(path, $"{DateTime.Now} - Data Received: {inData}" + "\n");

        DataReceived?.Invoke(this, inData);
    }

    public void Dispose()
    {
        _serialPort.DataReceived -= DataReceivedHandler;
        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
            File.AppendAllText(path, $"{DateTime.Now} - Port closed" + "\n");
        }
        _serialPort.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}