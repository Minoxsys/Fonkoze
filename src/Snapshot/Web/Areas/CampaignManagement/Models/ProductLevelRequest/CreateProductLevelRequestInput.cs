using System;
using System.Linq;

namespace Web.Areas.CampaignManagement.Models.ProductLevelRequest
{
    public class CreateProductLevelRequestInput
    {
        public Guid? ProductGroupId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? ScheduleId { get; set; }

        public ProductModel[] Products { get; set; }

        public class ProductModel
        {
            public Guid? Id { get; set; }
            public string SmsCode { get; set; }
            public string ProductItem { get; set; }
            public bool Selected { get; set; }

        } 
    }

   
}