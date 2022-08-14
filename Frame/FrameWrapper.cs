namespace Frame
{
    public class FrameWrapper
    {
        private byte[] _frameBuffer;
        private List<byte> _buffer = new List<byte>();

        public FrameWrapper()
        {
            _frameBuffer = new byte[Constants.FrameSize];
            _frameBuffer[0] = Constants.STX;
            _frameBuffer[Constants.FrameSize - 1] = Constants.ETX;
        }

        private int Index { get; set; }

        public List<byte> Data
        {
            get
            {
                return _buffer;
            }
            set
            {
                _buffer = value;
            }
        }

        public byte ChecksumCharacter
        {
            get { return _frameBuffer[46]; }
            private set
            {
                _frameBuffer[46] = value;
            }
        }

        public byte[] GetFrame()
        {
            ChecksumCharacter = Checksum.CalculateChecksum(_buffer);
            var l = _buffer.Count;

            for (int i = 0; i < Constants.DataBufferSize; i++)
            {
                if (i < l)
                {
                    _frameBuffer[i + 1] = _buffer[i];
                }
                else _frameBuffer[i + 1] = 0x0;
            }
            return _frameBuffer;
        }

        public void SetDataByte(byte data, int index)
        {
            _frameBuffer[index + 1] = data;
            _buffer.Add(data);
        }

        public void InitialisePacket()
        {
            _buffer.Clear();
        }
    }
}
