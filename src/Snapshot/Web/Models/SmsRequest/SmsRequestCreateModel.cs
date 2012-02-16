using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Models.SmsRequest
{
    public class SmsRequestCreateModel
    {
        [Required]
        public ReferenceModel Outpost { get; set; }
        [Required]
        public ReferenceModel ProductGroup { get; set; }
         
        public List<SelectListItem> Outposts { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }

        public SmsRequestCreateModel()
        {
            Outpost = new ReferenceModel();
            ProductGroup = new ReferenceModel();
        }
    }
}