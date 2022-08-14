using System.IO.Ports;

namespace CommsConsole.Helpers
{
    public class CommsBase
    {
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public Handshake Handshake { get; set; }
        public bool DtrEnable { get; set; }
        public int NodeAddress { get; set; }
        public int BaseAddress { get; set; }
        public string PortName { get; set; } = string.Empty;
    }
}
