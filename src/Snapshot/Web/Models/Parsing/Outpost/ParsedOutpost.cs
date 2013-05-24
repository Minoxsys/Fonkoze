using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Parsing.Outpost
{
    public class ParsedOutpost : IParsedOutpost
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string Name { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string ContactDetail { get; set; }

        public override bool Equals(object obj)
        {
            // Performs an equality check on two points (integer pairs). 
            if (obj == null || GetType() != obj.GetType()) return false;

            var o = (ParsedOutpost)obj;
            return Country == o.Country && Region == o.Region && District == o.District && Name == o.Name &&
                Longitude == o.Longitude && Latitude == o.Latitude && ContactDetail == o.ContactDetail;
        }
    }
}