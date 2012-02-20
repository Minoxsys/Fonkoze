using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Areas.CampaignManagement.Models.RequestSchedule
{
    public class RequestReminderOutputModel
    {
        public string PeriodType { get; set; }
        public int PeriodValue { get; set; }
    }

    public class RequestScheduleReferenceModel
    {
        public Guid Id { get; set; }
        public string ScheduleName { get; set; }
        public string Basis { get; set; }
        public string Frequency { get; set; }
        public List<RequestReminderOutputModel> Reminders { get; set; }
        public string CreationDate { get; set; }
    }
}