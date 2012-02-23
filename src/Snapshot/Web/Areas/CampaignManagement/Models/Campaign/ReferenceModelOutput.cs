using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class ReferenceModelOutput
    {
        public ReferenceModel[] Items { get; set; }
        public int TotalItems { get; set; }
    }
}