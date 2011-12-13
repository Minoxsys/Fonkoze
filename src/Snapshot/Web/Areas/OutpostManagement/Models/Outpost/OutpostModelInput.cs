using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostModelInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}