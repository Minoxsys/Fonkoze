using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictOverviewModel
    {
        public List<DistrictModel> Districts { get; set; }

        public string Error { get; set; }

        public DistrictOverviewModel()
        {
            this.Districts = new List<DistrictModel>();
        }
    }
}