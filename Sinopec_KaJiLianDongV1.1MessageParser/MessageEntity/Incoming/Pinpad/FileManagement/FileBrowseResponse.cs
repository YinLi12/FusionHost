using System.Collections.Generic;

namespace MessageParser
{
    public class FileBrowseResponse : MessageBase
    {
        public class FileBrowseResponseEntry
        {
            /// <summary>
            /// File unique identification number
            /// </summary>
            [Format(1, EncodingType.BIN, 0)]
            public byte FileId { get; set; }

            /// <summary>
            /// File signature (crc32)
            /// </summary>
            [EnumerableFormat(4, 1)]
            public List<byte> CRC { get; set; }

            /// <summary>
            /// File authentication flag (0,1,2)
            /// </summary>
            [Format(1, EncodingType.BIN, 2)]
            public byte Autenticated { get; set; }

            /// <summary>
            /// Ascii description of the file contents
            /// </summary>
            [Format(16, EncodingType.ASCII, 3)]
            public string Description { get; set; }
        }
        public enum fBrowseType
        {
            Message = 16,
            Images = 17,
            Font = 18,
            Log = 19,
            DisplayTemplate = 21,
            EMV_Table = 32,
            EMV_Message = 33,
            EMV_IssuerScript = 34,
            EMV_Log = 35,
            EMV_DataDump = 36,
            EMV_UserInterfaceStepList = 47
        }

        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        /// <summary>
        /// File usage and contents
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public fBrowseType FileType
        {
            get;
            set;
        }

        /// <summary>
        /// Number of files retrieved
        /// </summary>
        [Format(1, EncodingType.BIN, 2)]
        public byte NumEntries
        {
            get;
            set;
        }

        [EnumerableFormat("NumEntries", 3)]
        public List<FileBrowseResponseEntry> Entries { get; set; }

        /// <summary>
        /// In Kilobytes
        /// </summary>
        [Format(2, EncodingType.BIN, 4)]
        public int ResourceMemoryAvailable
        {
            get;
            set;
        }
    }
}
