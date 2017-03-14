namespace MessageParser
{
    /// <summary>
    /// SPOT sends requested file to Master
    /// </summary>
    public class FileUploadResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        [Format(4, EncodingType.BIN, 1)]
        public int FileSize { get; set; }
    }
}
