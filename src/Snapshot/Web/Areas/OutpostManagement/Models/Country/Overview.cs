
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class Overview
    {
        public List<CountryModel> Countries { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string Error { get; set; }
    }
}