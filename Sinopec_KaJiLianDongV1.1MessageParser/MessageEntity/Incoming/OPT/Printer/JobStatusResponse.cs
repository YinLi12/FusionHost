using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class JobStatusResponse : MessageBase
    {
        public enum PrinterJobStatus
        {
            JobCreated = 0x01,
            PrintCommandReceived = 0x02,
            Printing = 0x03,
            Success = 0x04,
            Failed = 0x05,
            Canceled = 0x06,
        }
        [Format(1, EncodingType.BIN, 0)]
        public OPTResultCode Ackcode { get; set; }

        [Format(4, EncodingType.BIN, 1)]
        public int JobId { get; set; }

        [Format(2, EncodingType.BIN, 2)]
        public int SeqNumber { get; set; }

        [Format(1, EncodingType.BIN, 3)]
        public PrinterJobStatus Status { get; set; }        
    }
}
