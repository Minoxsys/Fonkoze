using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Areas.CampaignManagement.Models.RequestSchedule
{
    public class RequestReminderInput
    {
        public string PeriodType { get; set; }
        public int PeriodValue { get; set; }
    }
    public class RequestScheduleInputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FrequencyType { get; set; }
        public int FrequencyValue { get; set; }
        public int StartOn { get; set; }
        public RequestReminderInput[] Reminders { get; set; }
    }
}