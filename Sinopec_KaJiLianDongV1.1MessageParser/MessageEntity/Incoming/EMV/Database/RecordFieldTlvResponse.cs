using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class RecordFieldTlvResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public EmvResultCode Ackcode { get; set; }
    }
}
