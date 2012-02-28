using System;
using System.Linq;

namespace Web.Areas.CampaignManagement.Models.ProductLevelRequest
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string ProductItem { get; set; }
        public string SmsCode { get; set; }
        public bool Selected { get; set; }
    }
}