using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Client;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Contact;


namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostInputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage="Name for the outpost is required")]
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string DetailMethod { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsWarehouse { get; set; }
        public OutpostModel Warehouse { get; set; }

        public RegionInputModel Region { get; set; }
        public DistrictInputModel District { get; set; }
        public ClientModel Client { get; set; }
        //public List<SelectListItem> Warehouses { get; set; }
         
        public class RegionInputModel
        {
           [Required(ErrorMessage = "Region is required")]
           public Guid Id { get; set; }
           public Guid CountryId { get; set; }
        }

        public class DistrictInputModel
        {
            [Required(ErrorMessage = "District is required")]
            public Guid Id { get; set; }
        }

        public class ClientInputModel
        {
            public Guid Id { get; set; }
        }
  
    }
}