using static Protocols.RS485;

namespace Protocols
{
    public interface IFrame
    {
        string Status { get; set; }

        event EventHandler<PacketReceivedEventArgs> PacketReceived;
        event EventHandler<PacketSentEventArgs> PacketSent;

        void SendData(List<byte> data);
    }
}
