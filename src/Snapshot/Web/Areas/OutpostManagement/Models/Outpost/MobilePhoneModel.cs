using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using Domain;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class MobilePhoneModel
    {
        public Guid Id { get; set; }
        public string MobileNumber { get; set; }
        public Guid Outpost_FK { get; set; }
        public Guid OutpostId { get; set; }
    }
}