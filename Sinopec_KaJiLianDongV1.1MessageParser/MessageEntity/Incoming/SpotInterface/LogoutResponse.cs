namespace MessageParser
{
    public class LogoutResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; private set; }
    }
}