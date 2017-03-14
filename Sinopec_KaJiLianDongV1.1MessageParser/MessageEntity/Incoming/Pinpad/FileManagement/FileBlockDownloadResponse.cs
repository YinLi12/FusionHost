namespace MessageParser
{
    public class FileBlockDownloadResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        /// <summary>
        /// Offset for next block transfe
        /// </summary>
        [Format(4, EncodingType.BIN, 1)]
        public int Offset { get; set; }
    }
}
