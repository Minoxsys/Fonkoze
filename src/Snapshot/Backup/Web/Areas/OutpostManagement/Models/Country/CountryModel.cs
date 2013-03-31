using System;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryModel
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string PhonePrefix { get; set; }
        public ClientModel Client { get; set; }
        public int RegionNo { get; set; }
        public Guid Id { get; set; }
    }
}