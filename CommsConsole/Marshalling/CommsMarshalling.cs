using CommsConsole.Helpers;
using CommsConsole.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CommsConsole.Marshalling
{

    public delegate void MarshallingNetworkRxd(string message);
    public delegate void MarshallingNetworkTxd(string message);

    public class CommsMarshalling : ICommsMarshalling
    {
        private readonly Exclusions _exclusions;
        private readonly SymbolicPorts _symbolicPorts;

        // Network related fields
        INetworkService _networkService;
        private string _networkPort = string.Empty;

        // General fields
        private List<string> _portList = new();
        private MarshallingNetworkRxd? _networkDataRxd;

        public CommsMarshalling(
            IOptions<Exclusions> exclusions
            , IOptions<SymbolicPorts> symbolicLinks
            , INetworkService networkService)
        {
            _exclusions = exclusions.Value;
            _symbolicPorts = symbolicLinks.Value;
            _networkService = networkService;
        }

        public List<string> EnumeratePorts()
        {
            var ports = _networkService.PortList().Except(_exclusions.Ports).ToList();
            if(_symbolicPorts.Ports.Count > 0)
            {
                ports.AddRange(_symbolicPorts.Ports);
            }
            return ports;
        }

        public bool OpenPort(string portName, int thisNodeAddress, MarshallingNetworkRxd marshallingNetworkRxd, MarshallingNetworkTxd marshallingNetworkTxd)
        {
            _networkService.SetThisNodeAddress(thisNodeAddress);
            _networkDataRxd = marshallingNetworkRxd;
            _networkPort = portName;
            _networkService.OpenPort(_networkPort, NetworkDataRxd, NetworkDataTxd);
            return true;
        }

        public void Send(string data, int targetNode)
        {
            _networkService.Send(data, targetNode);
        }

        public bool ClosePort()
        {
            if (_networkService != null)
            {
                return _networkService.ClosePort();
            }

            return false;
        }

        private void NetworkDataRxd(string message)
        {
            _networkDataRxd?.Invoke(message);
        }

        private void NetworkDataTxd(string message)
        {
            //TODO Implement later
        }
    }
}
