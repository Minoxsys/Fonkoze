using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Areas.CampaignManagement.Models.RequestSchedule
{
    public class RequestScheduleInputModel
    {
        public Guid Id { get; set; }
        public string ScheduleName { get; set; }
        public string Basis { get; set; }
        public virtual string FrequencyType { get; set; }
        public virtual int FrequencyValue { get; set; }
        public virtual int StartOn { get; set; }
        public List<RequestReminder> Reminders { get; set; }
    }
}