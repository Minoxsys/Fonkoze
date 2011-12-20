using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostModelOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string Email { get; set; }
        public string MainMobileNumber { get; set; }
    }
}