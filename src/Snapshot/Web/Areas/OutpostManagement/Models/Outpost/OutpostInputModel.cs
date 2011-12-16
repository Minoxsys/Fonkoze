using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostInputModel
    {
        [Required]
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string Email { get; set; }
        public string MainMobileNumber { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DistrictInput District { get; set; }
        public CountryInput Country { get; set; }
        public RegionInput Region { get; set; }
        public ClientModel Client { get; set; }
        public  IList<MobilePhone> MobilePhones { get; set; }
        public Guid Id { get; set; }

        public class CountryInput
        {
            public Guid Id { get; set; }
        }

        public class RegionInput
        {
            public Guid Id { get; set; }
        }

        public class DistrictInput
        {
            public Guid Id { get; set; }
        }
   }
}