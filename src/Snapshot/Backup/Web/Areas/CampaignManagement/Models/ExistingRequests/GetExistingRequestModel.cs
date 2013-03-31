namespace Web.Areas.CampaignManagement.Models.ExistingRequests
{
    public class GetExistingRequestModel
    {
        public string Campaign { get; set; }
        public string Outpost { get; set; }
        public string Date { get; set; }
        public string ProductGroup { get; set; }
        public int ProductsNo { get; set; }
    }
}