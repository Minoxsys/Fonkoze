using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryIndexOutputModel
    {
        public CountryModel[] Countries { get; set; }
        public int TotalItems { get; set; }

    }
}