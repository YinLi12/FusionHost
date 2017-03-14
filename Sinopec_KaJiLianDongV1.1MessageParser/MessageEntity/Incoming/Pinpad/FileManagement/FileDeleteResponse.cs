namespace MessageParser
{
    public class FileDeleteResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }
    }
}
