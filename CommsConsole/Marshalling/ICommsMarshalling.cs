using System.Collections.Generic;

namespace CommsConsole.Marshalling
{
    public interface ICommsMarshalling
    {
        bool OpenPort(string portName, int thisNodeAddress, MarshallingNetworkRxd marshallingNetworkRxd, MarshallingNetworkTxd marshallingNetworkTxd);
        List<string> EnumeratePorts();
        bool ClosePort();
        void Send(string data, int targetNode);
    }
}