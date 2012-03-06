using System;

namespace Web.Areas.CampaignManagement.Models.ExistingRequests
{
    public class GetExistingRequestsInput
    {
        public Guid? CampaignId { get; set; }
        public Guid? OutpostId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int? page { get; set; }
        public int? start { get; set; }
        public int? limit { get; set; }
    }
}