namespace Web.Services.SendEmail
{
    public class SmtpServerDetails
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string FromAddress { get; set; }
        public string FromPassword { get; set; }
    }
}