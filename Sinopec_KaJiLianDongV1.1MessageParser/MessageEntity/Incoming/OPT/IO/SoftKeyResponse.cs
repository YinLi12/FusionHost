using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
   public class SoftKeyResponse : MessageBase
    {
       public enum SoftKey
       {
           PumpButton1 = 0x01,
           PumpButton2 = 0x02,
           PumpButton3 = 0x03,
           PumpButton4 = 0x04,
           PumpButton5 = 0x05,
           PumpButton6 = 0x06,
           PumpButton7 = 0x07,
           PumpButton8 = 0x08,
           PumpButton9 = 0x09,
           PumpButton10 = 0x0A,
           SoftKey1 = 0x10,
           SoftKey2 = 0x11,
           SoftKey3 = 0x12,
           SoftKey4 = 0x13,
           SoftKey5 = 0x14,
           SoftKey6 = 0x15,
           SoftKey7 = 0x16,
           SoftKey8 = 0x17,
       }
       [Format(1, EncodingType.BIN, 0)]
       public OPTResultCode AckCode { get; set; }

       [Format(1, EncodingType.BIN, 1)]
       public SoftKey ButtonPressed { get; set; }
    }
}
