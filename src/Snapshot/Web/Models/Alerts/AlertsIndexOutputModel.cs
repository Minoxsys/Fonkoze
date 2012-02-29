using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Alerts
{
    public class AlertsIndexOutputModel
    {
        public AlertModel[] Alerts { get; set; }
        public int TotalItems { get; set; }
    }
}