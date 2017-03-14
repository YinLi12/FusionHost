using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class ENDEMVResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public EmvResultCode AckCode { get; set; }
    }
}
