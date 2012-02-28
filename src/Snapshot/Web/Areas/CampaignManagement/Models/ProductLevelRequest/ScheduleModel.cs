using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CampaignManagement.Models.ProductLevelRequest
{
    public class ScheduleModel
    {
        public string Id { get; set; }
        public string ScheduleName { get; set; }
        public string Basis { get; set; }
        public string Frequency { get; set; }
        public RequestReminderModel[] Reminders { get; set; }

        public bool Selected { get; set; }
    }
}