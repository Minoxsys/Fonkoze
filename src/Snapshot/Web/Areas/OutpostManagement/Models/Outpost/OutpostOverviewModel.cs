
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostOverviewModel
    {

        public List<OutpostModel> Outposts { get; set; }

        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Districts { get; set; }

        public string Error { get; set; }

        public IQueryService<Domain.Country> QueryCountry { get; set; }
        public IQueryService<Domain.Region> QueryRegion { get; set; }

         public OutpostOverviewModel()
        {
            this.Districts = new List<SelectListItem>();
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
        }

    }
}