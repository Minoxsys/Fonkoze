using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionInputModel
    {
        [Required]
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}