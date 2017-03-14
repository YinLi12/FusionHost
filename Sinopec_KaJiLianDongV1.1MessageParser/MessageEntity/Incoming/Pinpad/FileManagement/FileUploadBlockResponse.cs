using System.Collections.Generic;

namespace MessageParser
{
    /// <summary>
    /// SPOT sends requested file to Master
    /// </summary>
    public class FileUploadBlockResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        [Format(4, EncodingType.BIN, 1)]
        public int Offset { get; set; }

        [Range(1, 2048, "BlockSize value: {0} in FileUploadBlockResponse is not valid")]
        [Format(4, EncodingType.BIN, 2)]
        public int BlockSize { get; set; }

        /// <summary>
        /// Current data block transferred.
        /// </summary>
        [EnumerableFormat("BlockSize", 3)]
        public List<byte> BlockData { get; set; }
    }
}
