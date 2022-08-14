using Frame;
using System.IO.Ports;

namespace Protocols
{
    public class RS485 : IFrame
    {
        private FrameWrapper _outFrame;
        private FrameWrapper _inFrame;
        private SerialPort _serialPort;
        private byte _lastByte;
        private int _index;
        private bool _packetInFlight;

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;
        public event EventHandler<PacketSentEventArgs> PacketSent;
        public string Status { get; set; }

        public RS485(SerialPort serialPort)
        {
            _outFrame = new FrameWrapper();
            _inFrame = new FrameWrapper();
            Status = string.Empty;
            // Pass in a pre-configured Serial port instance
            _serialPort = serialPort;
            _serialPort.DataReceived += DataReceived;

        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var receiver = sender as SerialPort;
            byte inByte = Constants.ETX;
            while (receiver.IsOpen && receiver.BytesToRead > 0)
            {
                inByte = receiver.IsOpen ? Convert.ToByte(receiver.ReadByte()) : Constants.ETX;
                if (inByte == Constants.STX && !_packetInFlight)
                {
                    // Start transmission detected
                    _index = 0;
                    _packetInFlight = true;
                    _inFrame.InitialisePacket();
                }
                else if (inByte == Constants.ETX && _index + 2 >= Constants.FrameSize)
                {
                    _packetInFlight = false;
                    // End transmission detected
                    //_inFrame.SetDataByte('\0', _index-1 );
                    var ea = new PacketReceivedEventArgs { RawPacket = _inFrame.Data, TimeReached = DateTime.Now, Checksum = _lastByte };
                    OnPacketReceived(ea);
                }
                else
                {
                    if (_packetInFlight)
                    {
                        // Another databyte
                        if (_index + 2 < Constants.FrameSize)
                        {
                            _inFrame.SetDataByte(inByte, _index);
                            _lastByte = inByte;
                            _index++;
                        }
                    }
                }
            }
            if (!receiver.IsOpen)
            {
                _index = 0;
                _packetInFlight = false;
            }
        }

        public void SendData(List<byte> data)
        {
            _outFrame.Data = data;
            var x = _outFrame.GetFrame();
            if(_serialPort.IsOpen) _serialPort.Write(x, 0, Constants.FrameSize);
            var ea = new PacketSentEventArgs { Status = "Success", TimeReached = DateTime.Now };

            OnPacketSent(ea);
        }

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            EventHandler<PacketReceivedEventArgs> handler = PacketReceived;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPacketSent(PacketSentEventArgs e)
        {
            EventHandler<PacketSentEventArgs> handler = PacketSent;
            handler?.Invoke(this, e);
        }
    }
}