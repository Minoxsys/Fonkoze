
namespace Web.Areas.MessagesManagement.Models.ErrorRate
{
    public class ErrorRateViewModel
    {
        public string Sender { get; set; }
        public string SellerName { get; set; }
        public int ErrorMessages { get; set; }
        public int TotalMessages { get; set; }
        public int ErrorRate { get; set; }
    }
}