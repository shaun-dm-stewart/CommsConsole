using Frame;
using Protocols;
using System.Text;
using static Packet.GlobalConstants;

namespace Packet
{
    public class PacketHandler
    {
        private readonly IFrame _frameLayer;
        private List<List<byte>> _packetsToSend;
        private List<string> _packetsReceived;
        private byte _nodeAddress;
        private int _packetIndex;
        private byte _address;
        private List<byte> _acknowledgement;

        public event EventHandler<PacketReceivedEventArgs> PacketIn;
        public event EventHandler<PacketSentEventArgs> PacketOut;

        public PacketHandler(IFrame frameLayer, int nodeAddress)
        {
            _acknowledgement = new List<byte>();
            _frameLayer = frameLayer;
            _packetsToSend = new List<List<byte>>();
            _packetsReceived = new List<string>();
            _nodeAddress = Convert.ToByte(nodeAddress);
            _frameLayer.PacketReceived += PacketReceived;
            _frameLayer.PacketSent += PacketSent;
        }

        private void PacketSent(object sender, PacketSentEventArgs e)
        {
            //throw new NotImplementedException();
            var x = "Fred";
        }

        private void PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var msgType = ExtractMessageType(e.RawPacket);

            switch (msgType)
            {
                case 0:
                    // Do nothing, it's not for me
                    break;
                case 1:
                    // Send another packet if necessary
                    _packetIndex++;
                    if (_packetIndex < _packetsToSend.Count)
                    {
                        Transmit(_packetsToSend[_packetIndex]);
                    }
                    else
                    {
                        // We have sent the entire message so raise the message sent event
                    }
                    break;
                case 2:
                    // We just received an EOT flag so the incoming message is complete
                    HandOverReceivedMessage();
                    break;
                case 3:
                    // It's a new message
                    _packetsReceived.Clear();
                    break;
                case 4:
                    // It's a continuation
                    var packetToAdd = Encoding.ASCII.GetString(e.RawPacket.ToArray()).Substring(2, MAX_PACKET_SIZE + 1);
                    _packetsReceived.Add(packetToAdd.Replace("\0", ""));
                    break;
            }
        }

        public void SendPacket(DataPacket packetToSend)
        {
            var packetLength = packetToSend.Packet.Length;
            var numPackets = (packetLength / MAX_PACKET_SIZE) + ((packetLength % MAX_PACKET_SIZE) > 0 ? 1 : 0);
            var index = 0;

            _packetsToSend.Clear();
            _address = Convert.ToByte(packetToSend.Address);

            // The first packet contains the SOH flag and the message length in bytes
            var header = new List<byte>();
            header.Add(_address);
            header.Add(_nodeAddress);
            header.Add(Constants.SOH);
            var bytes = Encoding.ASCII.GetBytes($"{packetLength:D4}");
            header.AddRange(bytes);
            _packetsToSend.Add(header);

            for (int i = 0; i < numPackets; i++)
            {
                int biteSize;
                if (packetLength - index > MAX_PACKET_SIZE)
                {
                    biteSize = MAX_PACKET_SIZE;
                }
                else biteSize = (packetLength - index);

                var newSegment = new List<byte>();
                newSegment.Add(_address);
                newSegment.Add(_nodeAddress);
                var segmentData = Encoding.ASCII.GetBytes(packetToSend.Packet.Substring(index, biteSize));
                newSegment.AddRange(segmentData);
                _packetsToSend.Add(newSegment);
                index += MAX_PACKET_SIZE;
            }

            var trailer = new List<byte>();
            trailer.Add(_address);
            trailer.Add(_nodeAddress);
            trailer.Add(Constants.EOT);
            _packetsToSend.Add(trailer);

            _packetIndex = 0;
            Transmit(_packetsToSend[0]);
        }

        private void Transmit(List<byte> data)
        {
            // Do not increment anything here as we may have to retransmit
            _frameLayer.SendData(data);
        }

        private int ExtractMessageType(List<byte> data)
        {
            int destination = data[0];
            int source = data[1];
            int mType = data[2];
            var result = -1;
            if (destination == _nodeAddress)
            {
                // The message is addressed to me
                //if (source == _address)
                //{
                if (mType == Constants.ACK)
                {
                    // It's from the node I am transmitting to and it has acknowledged receipt of a packet
                    result = 1;
                }
                else if (mType == Constants.EOT)
                {
                    // It's an emd of transmission
                    result = 2;
                }
                else if (mType == Constants.SOH)
                {
                    // It's a new message
                    result = 3;
                }
                else
                {
                    // It's a continuation
                    result = 4;
                }
                //}
                //else
                //{
                //    //It's not for me so ignore it
                //    result = 0;
                //}
            }

            // Send an ack if called for
            if (result > 1)
            {
                _acknowledgement.Clear();
                _acknowledgement.Add(Convert.ToByte(source));
                _acknowledgement.Add(Convert.ToByte(destination));
                _acknowledgement.Add(Constants.ACK);

                Transmit(_acknowledgement);
            }
            return result;
        }

        private void HandOverReceivedMessage()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var segment in _packetsReceived)
            {
                sb.Append(segment);
            }

            var rea = new PacketReceivedEventArgs()
            {
                Data = sb.ToString(),
                TimeReached = DateTime.Now
            };

            OnPacketReceived(rea);
        }

        private void ConfirmTransmission()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var segment in _packetsToSend)
            {
                sb.Append(segment);
            }

            var tea = new PacketSentEventArgs()
            {
                Status = sb.ToString(),
                TimeReached = DateTime.Now
            };
        }

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            EventHandler<PacketReceivedEventArgs> handler = PacketIn;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPacketSent(PacketSentEventArgs e)
        {
            EventHandler<PacketSentEventArgs> handler = PacketOut;
            handler?.Invoke(this, e);
        }
    }
}
