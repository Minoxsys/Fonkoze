using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.District;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionOverviewModel
    {
        public List<RegionModel> Regions { get; set; }
       

        public RegionOverviewModel()
        {
            Regions = new List<RegionModel>();
            
        }
    }
}