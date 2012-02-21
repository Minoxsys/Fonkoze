using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class CampaignOverviewOutputModel
    {
        public List<CampaignOutputModel> Campaigns { get; set; }
        public int TotalItems { get; set; }

        public CampaignOverviewOutputModel()
        {
            this.Campaigns = new List<CampaignOutputModel>();
        }
    }
}