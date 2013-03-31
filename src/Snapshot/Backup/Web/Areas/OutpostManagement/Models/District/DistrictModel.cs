using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid RegionId { get; set; }
        public Guid CountryId { get; set; }
        public Guid ClientId { get; set; }
        public RegionModel Region { get; set; }
        public ClientModel Client { get; set; }
        public int OutpostNo { get; set; }

    }
}