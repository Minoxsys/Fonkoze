using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.StockAdministration.Models.StockItem
{
    public class StockItemInputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage="Name for Stock Item is required!")]
        public String Name { get; set; }
        public String Description { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public String SMSReferenceCode { get; set; }
        public StockItemOutputModel.OutpostModel Outpost { get; set; }
        public StockGroupInputModel StockGroup { get; set; }
        //public List<OutpostInputModel> Outposts { get; set; }

        public class StockGroupInputModel
        {
            public Guid Id { get; set; }
        }
        //public class OutpostInputModel
        //{
        //    public Guid Id {get; set;}
        //}

        public StockItemInputModel() 
        {
            this.StockGroup = new StockGroupInputModel();
            this.Outpost = new StockItemOutputModel.OutpostModel();
        }
    }
}