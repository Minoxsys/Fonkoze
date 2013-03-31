using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.LocationReport
{
    public class FilterModel
    {
        public Guid countryId { get; set; }
        public Guid regionId { get; set; }
        public Guid districtId { get; set; }
    }
}