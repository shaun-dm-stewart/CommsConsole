using Microsoft.Extensions.Options;
using CommsConsole.Helpers;
using System.Collections.Generic;

namespace CommsConsole.Services
{
    public class NetworkService : INetworkService
    {
        private readonly IPortManager _port;
        private DataTxd? _txd;
        private DataRxd? _rxd;

        public string PortName { get; set; } = string.Empty;

        public NetworkService(IOptions<NetworkSettings> networkSettings, IPortManager portManager)
        {
            _port = portManager;
            _port.PortConfig = networkSettings.Value;

        }

        public void SetThisNodeAddress(int thisNodeddress)
        {
            _port.PortConfig.NodeAddress = thisNodeddress;
        }

        public bool OpenPort(string portName, DataRxd dataReceived, DataTxd dataTransmitted)
        {
            return _port.OpenPort(portName, dataReceived, dataTransmitted);
        }

        public bool ClosePort()
        {
            return _port.ClosePort();
        }

        public void Send(string message, int? targetNode)
        {
            _port.Send(message, targetNode);
        }

        public List<string> PortList()
        {
            return _port.PortList();
        }
    }
}
