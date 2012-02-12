using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictIndexOutputModel
    {
        public List<DistrictModel> districts { get; set; }
        public int TotalItems { get; set; }

        public DistrictIndexOutputModel()
        {
            this.districts = new List<DistrictModel>();
        }
    }
}