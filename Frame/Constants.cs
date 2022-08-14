namespace Frame
{
    /// <summary>
    /// Contains the dimensions of the various frame components
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Comms characters
        /// </summary>
        public const byte SOH = 0x01;
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte EOT = 0x04;
        public const byte ENQ = 0x05;
        public const byte ACK = 0x06;
        public const byte DC1 = 0x11;

        private const int _charZerollowance = 1;
        /// <summary>
        /// Allowance for STX, checksum nd ETX.  One byte for each;
        /// </summary>
        private const int _frameOverhead = 3;
        /// <summary>
        /// The total frame size
        /// </summary>
        public const int FrameSize = 48;
        /// <summary>
        /// The size of the Frame less three bytes for STX, Checksum and ETX
        /// </summary>
        public const int DataBufferSize = FrameSize - _frameOverhead;
        /// <summary>
        /// The size of the Data buffer less 1 for the char 0
        /// </summary>
        public const int MaxBufferFill = DataBufferSize - _charZerollowance;
    }
}
