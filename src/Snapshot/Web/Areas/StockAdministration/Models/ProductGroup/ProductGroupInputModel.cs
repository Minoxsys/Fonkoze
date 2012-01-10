using System;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupInputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name for Product Group is required!")]
        public String Name { get; set; }
        public String Description { get; set; }
        public bool CreateCommingFromProduct { get; set; }
    }
}