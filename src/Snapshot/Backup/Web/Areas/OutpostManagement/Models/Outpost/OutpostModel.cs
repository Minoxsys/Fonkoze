using System;
using System.Collections.Generic;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Client;
using Web.Areas.OutpostManagement.Models.Contact;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostModel
    {
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string DetailMethod { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsWarehouse { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }
        public ContactModel Contact { get; set; }
        public OutpostModel Warehouse { get; set; }
        public List<OutpostModel> Outpost { get; set; }
        public List<Domain.Contact> Contacts { get; set; }
        public int ProductsNo { get; set; }
        public Guid Id { get; set; }
   }
}