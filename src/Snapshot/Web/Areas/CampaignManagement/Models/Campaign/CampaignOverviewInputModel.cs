using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class CampaignOverviewInputModel
    {
        public string sort { get; set; }
        public string dir { get; set; }
        public string SearchName { get; set; }

        public int? Page { get; set; }
        public int? Start { get; set; }
        public int? Limit { get; set; }
    }
}