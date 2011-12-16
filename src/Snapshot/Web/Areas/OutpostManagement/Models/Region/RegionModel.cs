using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Country;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionModel
    {
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public CountryModel Country { get; set; }
        public ClientModel Client { get; set; }
        public Guid Id { get; set; }
    }
}