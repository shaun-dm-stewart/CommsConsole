using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packet
{
    public class DataPacket
    {
        public int Address { get; set; }
        public string Packet { get; set; } = String.Empty;
    }
}
