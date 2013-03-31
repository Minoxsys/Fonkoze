using Web.Areas.CampaignManagement.Models.ExistingRequests;

namespace Web.Areas.CampaignManagement.Models.ExistingRequests
{
    public class GetExistingRequestsOutput
    {
        public int TotalItems { get; set; }
        public GetExistingRequestModel[] ExitingRequests { get; set; }
    }
}