
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.MobilePhone;

namespace Web.Areas.OutpostManagement.Models.MobilePhone
{
    public class MobilePhonesOverviewModel
    {
        public Guid OutpostId { get; set; }
        public Guid Outpost_FK { get; set; }
        public List<MobilePhoneModel> Items { get; set; }
    }
}