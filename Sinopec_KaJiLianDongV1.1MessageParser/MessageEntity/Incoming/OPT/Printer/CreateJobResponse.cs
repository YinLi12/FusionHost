using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class CreateJobResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public OPTResultCode AckCode { get; set; }

        [Format(4, EncodingType.BIN, 1)]
        public int JobId { get; set; }  
    }
}
