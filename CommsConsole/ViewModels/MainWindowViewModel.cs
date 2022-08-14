using Microsoft.Extensions.Options;
using CommsConsole.Helpers;
using CommsConsole.Marshalling;
using CommsConsole.Services;
using Protocols;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace CommsConsole.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly AppSettings? _settings;
        private readonly ICommsMarshalling? _commsMarshalling;

        public ReactiveCommand<Unit, Unit>? OpenPortCommand { get; private set; }
        public ReactiveCommand<Unit, Unit>? SendPacketCommand { get; private set; }
        public ReactiveCommand<Unit, Unit>? ClosePortCommand { get; private set; }
        public ReactiveCommand<Unit, Unit>? RefreshPortsCommand { get; private set; }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        private int _thisNode;
        public int ThisNode
        {
            get => _thisNode;
            set => this.RaiseAndSetIfChanged(ref _thisNode, value);
        }

        private int _targetNode;
        public int TargetNode
        {
            get => _targetNode;
            set => this.RaiseAndSetIfChanged(ref _targetNode, value);
        }

        private bool _portOpen;
        public bool PortOpen
        {
            get => _portOpen;
            set => this.RaiseAndSetIfChanged(ref _portOpen, value);
        }

        private List<string> _portList = new List<string>();
        public List<string> PortList
        {
            get => _portList;
            set => this.RaiseAndSetIfChanged(ref _portList, value);
        }

        private string _txPacket = string.Empty;
        public string TxPacket
        {
            get => _txPacket;
            set => this.RaiseAndSetIfChanged(ref _txPacket, value);
        }

        private string _rxPacket = string.Empty;
        public string RxPacket
        {
            get => _rxPacket;
            set => this.RaiseAndSetIfChanged(ref _rxPacket, value);
        }

        private string _appName = string.Empty;
        public string AppName
        {
            get => _appName;
            set => this.RaiseAndSetIfChanged(ref _appName, value);
        }

        private string _selectedPort = string.Empty;
        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPort, value);
            }
        }

        public MainWindowViewModel()
        {
            // This is needed to keep Avalonias designer happy
        }

        public MainWindowViewModel(IOptions<AppSettings> settings, ICommsMarshalling commsMarshalling)
        {
            _settings = settings.Value;
            Title = _settings.AppName;
            _commsMarshalling = commsMarshalling;
            Initialise();
        }

        private void Initialise()
        {
            if (_commsMarshalling != null)
            {
                PortList = _commsMarshalling.EnumeratePorts();
            }
            var canOpenPort = this.WhenAnyValue(
                x => x.PortOpen, x => x.SelectedPort, (isOpen, portName) => !PortOpen && !string.IsNullOrWhiteSpace(portName));

            var canClosePort = this.WhenAnyValue(
                x => x.PortOpen, x => x.SelectedPort, (isOpen, portName) => PortOpen);

            OpenPortCommand = ReactiveCommand.Create(OpenPort, canOpenPort);
            ClosePortCommand = ReactiveCommand.Create(ClosePort, canClosePort);
            SendPacketCommand = ReactiveCommand.Create(SendPacket, canClosePort);
            RefreshPortsCommand = ReactiveCommand.Create(RefreshPorts);
        }

        private void SerialPacketSent(object? sender, PacketSentEventArgs e)
        {
            var x = "Yippee";
        }

        private void SerialPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            RxPacket = e.Data;
        }

        private void SendPacket()
        {
            if (_commsMarshalling != null) _commsMarshalling.Send(_txPacket, TargetNode);
        }
        private void ClosePort()
        {
            if (_commsMarshalling != null)
            {
                if (_commsMarshalling.ClosePort()) PortOpen = false;
            }
        }

        private void OpenPort()
        {
            PortOpen = false;
            if (_commsMarshalling == null) return;
            PortOpen = _commsMarshalling.OpenPort(SelectedPort, ThisNode, DataReceived, DataTransmitted);
        }

        private void RefreshPorts()
        {
            PortList.Clear();
            if (_commsMarshalling == null) return;
            PortList = _commsMarshalling.EnumeratePorts();
        }

        void DataReceived(string message)
        {
            RxPacket = message;
        }

        void DataTransmitted(string message)
        {
            var x = message;
        }
    }
}
