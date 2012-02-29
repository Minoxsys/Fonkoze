using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Alerts
{
    public class AlertModel
    {
        public Guid Id { get; set; }
        public string OutpostName { get; set; }
        public Guid OutpostId { get; set; }
        public string Contact { get; set; }
        public string ProductGroupName { get; set; }
        public Guid ProductGroupId { get; set; }
        public string LowLevelStock { get; set; }
        public string LastUpdate { get; set; }
    }
}