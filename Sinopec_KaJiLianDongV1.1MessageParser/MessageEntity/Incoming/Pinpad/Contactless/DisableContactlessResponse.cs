using System.Collections.Generic;

namespace MessageParser
{
    public class DisableContactlessResponse : MessageBase
    {
        public enum DisableActionCode
        {
            OK = 0x00,
            KO = 0x01,
        }

        [Format(1, EncodingType.BIN, 0)]
        public DisableActionCode ActionCode { get; set; }

        [EnumerableFormat(2, 1)]
        public List<byte> PcdResult { get; set; }
    }
}
