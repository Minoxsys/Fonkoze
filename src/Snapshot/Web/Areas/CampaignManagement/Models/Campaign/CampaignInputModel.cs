using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class CampaignInputModel
    {
        public string CampaignName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CountriesIds { get; set; }
        public string RegionsIds { get; set; }
        public string DistrictsIds { get; set; }
        public string OutpostsIds { get; set; }

    }
}