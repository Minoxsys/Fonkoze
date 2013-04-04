namespace Web.Services
{
    public class RawSmsReceivedParseResult
    {
        public virtual SmsReceived SmsReceived { get; internal set; }
        public virtual bool ParseSucceeded { get; set; }
        public virtual string ParseErrorMessage { get; internal set; }
    }
}