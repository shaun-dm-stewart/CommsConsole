using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    public class Checksum
    {
        //calculates the checksum from the byte[] 
        public static byte CalculateChecksum(List<byte> byteData)
        {
            Byte chkSumByte = 0x00;
            for (int i = 0; i < byteData.Count; i++)
                chkSumByte ^= byteData[i];
            return chkSumByte;
        }
    }
}
