using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RegionModel Region { get; set; }
        public ClientModel Client { get; set; }
    }
}