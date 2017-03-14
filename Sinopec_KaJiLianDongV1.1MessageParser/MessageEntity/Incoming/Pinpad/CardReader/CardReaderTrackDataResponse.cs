using System.Collections.Generic;

namespace MessageParser
{
    public class TrackDataResponse : MessageBase
    {
        public class Data
        {
            public enum Source
            {
                Track1 = 0x01,
                Track2 = 0x02,
                Track3 = 0x04,
                ATR = 0x08,
                ICCData = 0x10,
                TransactionToken = 0x80,
            }

            public enum Status
            {
                TrackOrICCDataOK = 0x00,
                DataNotEncodedOrChipNotPresent = 0x01,
                SSError = 0x02,
                ESError = 0x04,
                LRCError = 0x08,
                URCError = 0x10,
                NoDataFound = 0x20,
                JitterError = 0x40,
                GenericError = 0x80,
            }

            [Format(1, EncodingType.BIN, 0)]
            public Source DataSource { get; set; }

            [Format(1, EncodingType.BIN, 1)]
            public Status DataStatus { get; set; }

            //[Range(0, 65535, "Current size: {0} exceed, must between {1} and {2}")]
            [Format(2, EncodingType.BIN, 2)]
            public int Size { get; set; }

            [EnumerableFormat("Size", 3)]
            public List<byte> TrackOrChipData { get; set; }
        }

        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        [Format(1, EncodingType.BIN, 1)]
        public byte NumTracks { get; set; }

        [EnumerableFormat("NumTracks", 2)]
        public List<Data> CardData { get; set; }
    }
}
