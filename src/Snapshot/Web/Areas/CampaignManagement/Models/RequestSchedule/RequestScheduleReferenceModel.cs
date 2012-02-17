using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Areas.CampaignManagement.Models.RequestSchedule
{
    public class RequestScheduleReferenceModel
    {
        public Guid Id { get; set; }
        public string ScheduleName { get; set; }
        public string Basis { get; set; }
        public string Frequency { get; set; }
        public List<RequestReminder> Reminders { get; set; }
        public DateTime CreationDate { get; set; }
    }
}