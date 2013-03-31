using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Models.EmailRequest
{
    public class EmailCreateModel
    {
        [Required]
        public ReferenceModel Outpost { get; set; }
        [Required]
        public ReferenceModel ProductGroup { get; set; }

        public List<SelectListItem> Outposts { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }

        public EmailCreateModel()
        {
            Outpost = new ReferenceModel();
            ProductGroup = new ReferenceModel();
        }
    }
}