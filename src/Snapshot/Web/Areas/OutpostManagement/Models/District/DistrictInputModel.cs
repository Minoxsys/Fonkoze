using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictInputModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public RegionModel Region { get; set; }
        public ClientModel Client { get; set; }
    }
}