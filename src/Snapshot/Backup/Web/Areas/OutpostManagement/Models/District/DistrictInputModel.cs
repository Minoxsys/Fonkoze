using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictInputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage="Name for district is required")]
        public string Name { get; set; }
        public RegionInputModel Region { get; set; }
        public ClientInputModel Client { get; set; }

        public DistrictInputModel()
        {
            this.Region = new RegionInputModel();
            this.Client = new ClientInputModel();
        }
        public class RegionInputModel
        {
            [Required(ErrorMessage = "Region is required")]
            public Guid Id { get; set; }

            public Guid CountryId { get; set; }
        }

        public class ClientInputModel
        {
            public Guid Id { get; set; }
        }
    }

   
}