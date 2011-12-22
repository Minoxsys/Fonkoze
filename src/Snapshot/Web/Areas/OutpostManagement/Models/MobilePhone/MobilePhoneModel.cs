using System;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.MobilePhone
{
    public class MobilePhoneModel
    {
        public Guid Id { get; set; }
        public string MethodType { get; set; }
        public string ContactDetail { get; set; }
        public string MainMethod { get; set; }
        public Guid Outpost_FK { get; set; }
        //public Guid Client_FK { get; set; }
    }
}