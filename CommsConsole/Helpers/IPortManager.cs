using CommsConsole.Services;
using Protocols;
using System.Collections.Generic;

namespace CommsConsole.Helpers
{
    public interface IPortManager
    {
        public CommsBase PortConfig { get; set; }
        bool IsPortOpen { get; }
        bool ClosePort();
        bool OpenPort(string portName, DataRxd dataReceived, DataTxd dataTransmitted);
        List<string> PortList();
        void Send(string message, int? targetNode);
    }
}