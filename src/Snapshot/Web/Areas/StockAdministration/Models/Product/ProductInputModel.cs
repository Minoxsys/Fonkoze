using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductInputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage="Name for Stock Item is required!")]
        public String Name { get; set; }
        public String Description { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public String SMSReferenceCode { get; set; }
        public ProductGroupInputModel ProductGroup { get; set; }
               
        public class ProductGroupInputModel
        {
            public Guid Id { get; set; }
        }
       
        public ProductInputModel() 
        {
            this.ProductGroup = new ProductGroupInputModel();
         
        }
    }
}