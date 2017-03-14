using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class OPTSystemStatusResponse : MessageBase
    {
        public class Entry
        {
            public enum SubsystemType
            {
                Printer = 0x01,
                IOModule = 0x02,
                Trind = 0x04,
                BNA = 0x05,
            }
            public enum PrinterStatus
            {
                Error = 0x01,
                Ready = 0x02,
                Busy = 0x03,
                Startup = 0x04,
            }
            public enum PrinterExtdStatus
            {
                NoInfo = 0x00,
                TicketPresent = 0x01,
                PaperWarning = 0x02,
            }
            public enum PrinterErrors
            {
                NoError = 0x00,
                NotResponding = 0x01,
                PaperEnd = 0x02,
                HeadUp = 0x04,
                HardwareError = 0x08,
                PaperJam = 0x16,
            }
            public enum IOModuleStatus
            {
                Error = 0x01,
                Ready = 0x02
            }
            public enum IOModuleExtStatus
            {
                None = 0x00,
                SafeboxOpened = 0x01,
            }
            public enum IOModuleErrors
            {
                Ok = 0x00,
            }

            [Format(1, EncodingType.BIN, 0)]
            public SubsystemType DeviceCode { get; set; }

            [Format(1, EncodingType.BIN, 1)]
            public byte Status { get; set; }
            
            [Format(1, EncodingType.BIN, 2)]
            public byte StatusExt { get; set; }

            [Format(1, EncodingType.BIN, 3)]
            public byte ErrorCode { get; set; }
        }

        [Format(1, EncodingType.BIN, 0)]
        public OPTResultCode AckCode { get; set; }

        /// <summary>
        /// Gets the total NumEntries count
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public byte NumEntries { get; set; }

        [EnumerableFormat("NumEntries", 2)]
        public List<Entry> Entries { get; set; }
    }
}
