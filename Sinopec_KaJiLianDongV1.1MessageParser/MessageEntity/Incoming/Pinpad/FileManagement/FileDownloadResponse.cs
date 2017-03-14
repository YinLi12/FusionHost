namespace MessageParser
{
    public class FileDownloadResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        /// <summary>
        /// In case of resumed incomplete transfer it provides the append position.
        /// </summary>
        [Format(4, EncodingType.BIN, 1)]
        public int Offset { get; set; }
    }
}
