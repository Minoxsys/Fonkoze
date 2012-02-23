using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class CampaignModel
    {
        public String Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool Opened { get; set; }
        public byte[] Options { get; set; }
        public ClientModel Client { get; set; }
    }
}