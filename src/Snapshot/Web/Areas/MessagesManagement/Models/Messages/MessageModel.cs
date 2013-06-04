namespace Web.Areas.MessagesManagement.Models.Messages
{
    public class MessageModel
    {
        public string Sender { get; set; }
        public string OutpostName { get; set; }
        public string Date { get; set; }
        public string Content { get; set; }
        public bool ParseSucceeded { get; set; }
        public string ParseErrorMessage { get; set; }
    }
}