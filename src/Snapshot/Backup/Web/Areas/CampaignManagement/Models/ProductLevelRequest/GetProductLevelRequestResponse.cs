using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.ProductLevelRequest
{
    public class GetProductLevelRequestResponse
    {
        public int TotalItems { get; set; }
        public GetProductLevelRequestModel[] ProductLevelRequests { get; set; }
    }
}