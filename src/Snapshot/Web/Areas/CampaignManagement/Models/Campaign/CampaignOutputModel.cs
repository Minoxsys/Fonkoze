using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class CampaignOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public String CreationDate { get; set; }
        public String Status { get; set; }
        public String Options { get; set; }
    }
}