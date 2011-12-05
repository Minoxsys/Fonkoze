using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class EventsCalendarModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public DateTime Date
        {
            get
            {
                return new DateTime(Year, Month, Day);
            }
        }
        public List<dynamic> Events { get; set; }


        public EventsCalendarModel()
        {
            Year = DateTime.Today.Year;
            Month = DateTime.Today.Month;
            Day = DateTime.Today.Day;

            

            Events = new List<dynamic>();
        }


    }
}