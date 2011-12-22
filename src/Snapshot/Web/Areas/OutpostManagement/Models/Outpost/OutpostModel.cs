using System;
using System.Collections.Generic;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostModel
    {
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string MainMethod { get; set; }
        public string DetailMethod { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }
        public IList<Domain.MobilePhone> MobilePhones { get; set; }
        public int StockItemsNo { get; set; }
        public Guid Id { get; set; }
   }
}