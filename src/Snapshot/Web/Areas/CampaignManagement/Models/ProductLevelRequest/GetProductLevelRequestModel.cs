using System;
using System.Linq;

namespace Web.Areas.CampaignManagement.Models.ProductLevelRequest
{
    public class GetProductLevelRequestModel
    {
        public string Campaign { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string ScheduleName { get; set; }
        public string Frequency { get; set; }
        public string ProductGroup{get;set;}
        public string ProductSmsCodes { get; set; }

        public bool Editable { get; set; }

    }
}