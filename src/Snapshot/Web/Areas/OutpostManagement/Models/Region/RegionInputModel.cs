using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Country;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionInputModel
    {
        [Required]
        public string Name { get; set; }
        public CountryModel Country {get; set;}
        public Guid Id { get; set; }
    }
}