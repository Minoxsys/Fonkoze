using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Alerts
{
    public class AlertsIndexModel
    {
        public string _dc { get; set; }

        public int? page { get; set; }

        public int? start { get; set; }
        public int? limit { get; set; }

        public string sort { get; set; }
        public string dir { get; set; }

        public string searchValue { get; set; }
    }
}