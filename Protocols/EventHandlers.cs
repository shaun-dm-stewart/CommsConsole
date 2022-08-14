namespace Protocols
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public List<byte>? RawPacket { get; set; } = new();
        public string Data { get; set; } = String.Empty;
        public DateTime TimeReached { get; set; }
        public byte Checksum { get; set; }
    }

    public class PacketSentEventArgs : EventArgs
    {
        public string Status { get; set; } = String.Empty;
        public DateTime TimeReached { get; set; }
    }
}
