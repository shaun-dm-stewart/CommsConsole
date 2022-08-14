using System.Collections.Generic;

namespace CommsConsole.Services
{
    public interface IPortService
    {
        void SetThisNodeAddress(int thisNodeAddress);
        string PortName { get; set; }
        bool ClosePort();
        bool OpenPort(string portName, DataRxd dataReceived, DataTxd dataTransmitted);
        List<string> PortList();
        void Send(string message, int? targetNode = null);
    }
}