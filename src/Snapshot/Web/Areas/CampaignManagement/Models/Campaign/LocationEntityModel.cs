using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.Campaign
{
    public class LocationEntityModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String BelongsTo_LocationEntityName { get; set; }
    }
}