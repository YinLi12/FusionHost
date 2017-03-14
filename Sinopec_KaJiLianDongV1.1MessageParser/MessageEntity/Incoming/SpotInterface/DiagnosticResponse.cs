namespace MessageParser
{
    public class DiagnosticResponse : MessageBase
    {
        [Range(1, 2, "only 1 or 2 is applicable for ErrorCode")]
        [Format(1, EncodingType.BIN, 0)]
        public int ErrorCode { get; set; }
    }
}