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
        public string DetailMethod { get; set; }
        public string Latitude { get; set; }
        //^[NS]([0-8][0-9](\.[0-5]\d){2}|90(\.00){2})\040[EW]((0\d\d|1[0-7]\d)(\.[0-5]\d){2}|180(\.00){2})$        
        public string Longitude { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }
        public IList<Domain.Contact> Contacts { get; set; }
        public int ProductsNo { get; set; }
        public Guid Id { get; set; }
   }
}