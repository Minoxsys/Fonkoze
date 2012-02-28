using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class ReferenceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}