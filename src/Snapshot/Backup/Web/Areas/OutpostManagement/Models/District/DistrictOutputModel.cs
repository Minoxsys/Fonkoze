using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using System.Web.Mvc;
using Core.Persistence;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public Guid RegionId { get; set; }
       
        public DistrictOutputModel() { }
       
    }
}