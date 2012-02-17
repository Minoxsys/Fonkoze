using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Models.RequestScheduleManager
{
    public class RequestScheduleListForJsonOutput
    {
        public RequestScheduleReferenceModel[] RequestSchedules { get; set; }
        public int TotalItems { get; set; }
    }
}