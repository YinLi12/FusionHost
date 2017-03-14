using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class CustomerInputEnableResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        [Format(2, EncodingType.BIN, 1)]
        public int InputSize { get; set; }

        [EnumerableFormat("InputSize", 2, EncodingType = EncodingType.BIN)]
        public List<byte> InputData { get; set; }

        [Format(2, EncodingType.BIN, 3)]
        public int KeySize { get; set; }

        [EnumerableFormat("KeySize", 4, EncodingType = EncodingType.BIN)]
        public List<byte> KeyData { get; set; }

        [EnumerableFormat(10, 5, EncodingType = EncodingType.BIN)]
        public List<byte> SMIDR { get; set; }
    }
}
