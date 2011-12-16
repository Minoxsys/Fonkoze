
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OverviewPhones
    {
        public Guid OutpostId { get; set; }
        public Guid Outpost_FK { get; set; }
        public List<MobilePhoneModel> Items { get; set; }
    }
}