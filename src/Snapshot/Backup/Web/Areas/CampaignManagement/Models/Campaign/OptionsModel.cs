using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class OptionsModel
    {
        public string Countries { get; set; }
        public string Regions { get; set; }
        public string Districts { get; set; }
        public string Outposts { get; set; }
    }
}