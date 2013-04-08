namespace Web.ReceiveSmsUseCase.Models
{
    public class ReceivedSmsInputModel
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string InNumber { get; set; }
        public string Email { get; set; }
        public string Credits { get; set; }
    }
}
