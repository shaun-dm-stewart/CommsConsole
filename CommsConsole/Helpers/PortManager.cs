using Avalonia.Threading;
using Packet;
using CommsConsole.Helpers;
using Protocols;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace CommsConsole.Services
{
    public delegate void DataRxd(string message);
    public delegate void DataTxd(string message);

    public class PortManager : IPortManager
    {
        SerialPort _serialPort = new SerialPort();
        RS485? _xceiver;
        PacketHandler? _packetLayer;
        DataRxd? _dataReceived;
        DataTxd? _dataTransmitted;

        public PacketHandler? PacketLayer
        {
            get { return _packetLayer; }
        }

        public bool IsPortOpen
        {
            get { return _serialPort.IsOpen; }
        }

        public PortManager()
        {
            PortConfig = new CommsBase();
        }

        public CommsBase PortConfig { get; set; }

        public bool OpenPort(string portName, DataRxd dataReceived, DataTxd dataTransmitted)
        {
            try
            {
                _dataReceived = dataReceived;
                _dataTransmitted = dataTransmitted;
                if (_serialPort.IsOpen) _serialPort.Close();
                _serialPort.PortName = portName;
                _serialPort.BaudRate = PortConfig.BaudRate;
                _serialPort.DataBits = PortConfig.DataBits;
                _serialPort.StopBits = PortConfig.StopBits;
                _serialPort.Parity = PortConfig.Parity;
                _serialPort.Handshake = PortConfig.Handshake;
                _serialPort.DtrEnable = PortConfig.DtrEnable;

                _serialPort.Open();
                _xceiver = new RS485(_serialPort);
                _packetLayer = new PacketHandler(_xceiver, PortConfig.NodeAddress);
                _packetLayer.PacketIn += PacketRxd;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool ClosePort()
        {
            if (_serialPort.IsOpen) _serialPort.Close();
            return true;
        }

        public List<string> PortList()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public void Send(string message, int? nodeAddress)
        {
            var nodeId = PortConfig.BaseAddress;
            if(nodeAddress != null)
            {
                nodeId = (int)nodeAddress;
            }

            if (_packetLayer != null)
            {
                var pkt = new DataPacket
                {
                    Address = nodeId,
                    Packet = message
                };
                _packetLayer.SendPacket(pkt);
            }
        }

        #region Event stuff

        private void PacketRxd(object sender, PacketReceivedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _dataReceived(e.Data);
            });
        }

        #endregion
    }
}
