namespace MessageParser
{
    public class ParsingEventArg<TFrom, TTo> : System.EventArgs
    {
        public TFrom From { get; set; }
        public TTo To { get; set; }
    }
}
